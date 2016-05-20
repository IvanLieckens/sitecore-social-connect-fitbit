using System;

namespace Sitecore.Social.Fitbit.Model
{
    public class LifetimeBest<T>
    {
        public DateTime Date { get; set; }

        public T Value { get; set; }
    }
}
