namespace Sale.Fronrend.Helpers
{
    public class MultipleSelectorModel
    {
        public string Key { get; set; } = null!;

        public string Value { get; set; } = null!;


        public MultipleSelectorModel(string key , string value)
        {
            Key = key;
            Value = value;
        }
    }
}
