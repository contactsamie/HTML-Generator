namespace uig.lib
{
    public class Sources
    {
        public string JQuery { set; get; }
    }


    public class SourcePaths
    {
        public SourcePaths(string scripts, string styles, string images, string libraries)
        {
            Scripts = scripts.TrimEnd('/').TrimEnd('\\');
            Styles = styles.TrimEnd('/').TrimEnd('\\');
            Images = images.TrimEnd('/').TrimEnd('\\');
            Libraries = libraries.TrimEnd('/').TrimEnd('\\');
        }

        public string Scripts { private set; get; }
        public string Styles { private set; get; }

        public string Images { private set; get; }

        public string Libraries { private set; get; }
    }
}