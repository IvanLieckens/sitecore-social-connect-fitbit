using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;

using Sitecore.Diagnostics;
using Sitecore.Reflection;
using Sitecore.Social.Fitbit.Helpers;
using Sitecore.Social.Fitbit.Model;
using Sitecore.Social.Infrastructure.Utils;
using Sitecore.Social.NetworkProviders;
using Sitecore.Social.NetworkProviders.Connector.Paths;
using Sitecore.Social.NetworkProviders.NetworkFields;
using Sitecore.Text;
using Sitecore.Web;

namespace Sitecore.Social.Fitbit.Networks.Providers
{
    /// <summary>
    /// Fitbit Network Provider for Sitecore Social Connect
    /// </summary>
    /// <remarks>
    /// With help from https://github.com/aarondcoleman/Fitbit.NET/blob/async-portable/Fitbit.Portable/OAuth2/OAuth2Helper.cs
    /// </remarks>
    public class FitbitNetworkProvider : NetworkProvider, NetworkProviders.Interfaces.IAuth, NetworkProviders.Interfaces.IGetAccountInfo, NetworkProviders.Interfaces.IAccessTokenSecretRenewal
    {
        #region Fields

        private string _fitbitAuthUrl = "https://www.fitbit.com/oauth2/authorize";

        private string _fitbitTokenUrl = "https://api.fitbit.com/oauth2/token";

        private string _fitbitProfileUrlFormat = "https://api.fitbit.com/1/user/{0}/profile.json";

        private string _fitbitScope = "profile activity heartrate";

        #endregion Fields

        public FitbitNetworkProvider(NetworkProviders.Application application)
            : base(application)
        {
        }

        #region Properties

        public string FitbitAuthUrl
        {
            get
            {
                return _fitbitAuthUrl;
            }

            set
            {
                _fitbitAuthUrl = value;
            }
        }

        public string FitbitTokenUrl
        {
            get
            {
                return _fitbitTokenUrl;
            }

            set
            {
                _fitbitTokenUrl = value;
            }
        }

        public string FitbitProfileUrlFormat
        {
            get
            {
                return _fitbitProfileUrlFormat;
            }

            set
            {
                _fitbitProfileUrlFormat = value;
            }
        }

        public string FitbitScope
        {
            get
            {
                return _fitbitScope;
            }

            set
            {
                _fitbitScope = value;
            }
        }

        #endregion Properties

        #region IAuth Implementation

        /// <summary>
        /// The AuthGetCode method must compose an authorization URL and redirect users to the social network.
        /// The AuthGetAccessToken method must get the OAuth access token from the social network.
        /// </summary>
        public void AuthGetCode(NetworkProviders.Args.AuthArgs args)
        {
            UrlString authUrl = new UrlString(FitbitAuthUrl);
            authUrl.Add("client_id", args.Application.ApplicationKey);
            authUrl.Add("response_type", "code");
            authUrl.Add("scope", FitbitScope);
            authUrl.Add("state", args.StateKey.ToString());
            string callbackUrl = WebUtil.GetFullUrl(string.Format("{0}?type=access", Paths.SocialLoginHandlerPath));
            authUrl.Add("redirect_uri", callbackUrl);

            RedirectUtil.Redirect(authUrl.ToString());
        }

        /// <summary>
        /// The AuthGetAccessToken method must get the OAuth access token from the social network and 
        /// then call the InvokeAuthCompleted method of the base class 
        /// passing the initialized instance of AuthCompletedArgs.
        /// </summary>
        public void AuthGetAccessToken(NetworkProviders.Args.AuthArgs args)
        {
            OAuth2Token token = ExchangeAuthCodeForAccessToken(args, HttpContext.Current.Request["code"]);

            if (args.CallbackType == null)
            {
                return;
            }
            
            NetworkProviders.Args.AuthCompletedArgs authCompletedArgs = new NetworkProviders.Args.AuthCompletedArgs
            {
                Application = args.Application,
                AccessTokenSecret = token.Token,
                AccessTokenSecretExpirationDate = DateTime.UtcNow.AddSeconds(token.ExpiresIn),
                AccessTokenSecretIssueDate = DateTime.UtcNow,
                RefreshToken = token.RefreshToken,
                CallbackPage = args.CallbackUrl,
                ExternalData = args.ExternalData,
                AttachAccountToLoggedInUser = args.AttachAccountToLoggedInUser,
                IsAsyncProfileUpdate = args.IsAsyncProfileUpdate
            };

            InvokeAuthCompleted(args.CallbackType, authCompletedArgs);
        }

        #endregion IAuth Implementation

        #region IGetAccountInfo Implementation

        public string GetDisplayName(Account account)
        {
            ProfileInfo currentProfile = GetCurrentUserProfile(account);

            return currentProfile.DisplayName;
        }

        public string GetAccountId(Account account)
        {
            ProfileInfo currentProfile = GetCurrentUserProfile(account);

            return currentProfile.EncodedId;
        }

        public AccountBasicData GetAccountBasicData(Account account)
        {
            ProfileInfo currentProfile = GetCurrentUserProfile(account);

            return new AccountBasicData
            {
                Account = account,
                FullName = currentProfile.FullName,
                Id = currentProfile.EncodedId
            };
        }

        public IEnumerable<Field> GetAccountInfo(Account account, IEnumerable<FieldInfo> acceptedFields)
        {
            List<FieldInfo> acceptedFieldsList = acceptedFields.ToList();
            if (!acceptedFieldsList.Any())
            {
                yield break;
            }

            ProfileInfo currentProfile = GetCurrentUserProfile(account);
            foreach (var acceptedField in acceptedFieldsList.Where(acceptedField => !string.IsNullOrEmpty(acceptedField.OriginalKey)))
            {
                System.Reflection.PropertyInfo propertyInfo = ReflectionUtil.GetPropertyInfo(currentProfile, acceptedField.OriginalKey);
                if (propertyInfo == null)
                {
                    Log.Warn(string.Format(CultureInfo.CurrentCulture, "There is no property \"{0}\" in a UserInfo object", acceptedField.OriginalKey), this);
                    continue;
                }

                object propertyValue = propertyInfo.GetValue(currentProfile);
                if (propertyValue == null)
                {
                    continue;
                }

                yield return new Field
                {
                    Name = GetFieldSitecoreKey(acceptedField),
                    Value = propertyValue.ToString()
                };
            }
        }

        #endregion IGetAccountInfo Implementation

        #region IAccessTokenSecretRenewal Implementation

        public void RefreshAccessTokenSecret(Account account)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", account.RefreshToken)
            });

            OAuth2Token token = RequestHelper.ExecuteRequest<OAuth2Token>(FitbitTokenUrl, account.Application, null, content);

            account.AccessTokenSecret = token.Token;
            account.AccessTokenSecretExpirationDate = DateTime.UtcNow.AddSeconds(token.ExpiresIn);
            account.AccessTokenSecretIssueDate = DateTime.UtcNow;
            account.RefreshToken = token.RefreshToken;
        }

        #endregion IAccessTokenSecretRenewal Implementation

        #region Private Methods

        private ProfileInfo GetCurrentUserProfile(Account account)
        {
            string currentUserProfileUrl = string.Format(FitbitProfileUrlFormat, "-");
            ProfileResponse profile = RequestHelper.ExecuteRequest<ProfileResponse>(currentUserProfileUrl, account);

            return profile.User;
        }

        private OAuth2Token ExchangeAuthCodeForAccessToken(NetworkProviders.Args.AuthArgs args, string code)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", args.Application.ApplicationKey),
                new KeyValuePair<string, string>("redirect_uri", WebUtil.GetFullUrl(string.Format("{0}?type=access", Paths.SocialLoginHandlerPath))),
                new KeyValuePair<string, string>("state", args.StateKey.ToString())
            });

            OAuth2Token result = RequestHelper.ExecuteRequest<OAuth2Token>(FitbitTokenUrl, args.Application, null, content);

            return result;
        }

        #endregion Private Methods
    }
}
