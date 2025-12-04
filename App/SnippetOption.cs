namespace fvm
{
    public class SnippetOption
    {
        public string Name { get; set; } = "";
        public List<string> Tags { get; set; } = new();

        public bool Matches(string resourceBase, string frontend)
        {
            if (Tags.Contains("ls") && resourceBase is not "lsModule" and not "lsResource")
                return false;

            if (Tags.Contains("esx") && resourceBase != "ESX")
                return false;

            if (Tags.Contains("react")&& frontend !="React")
                return false;

            return true;
        }
    }
}
