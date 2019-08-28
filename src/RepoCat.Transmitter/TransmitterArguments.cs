using System;

namespace RepoCat.Transmitter
{
    public class TransmitterArguments
    {

        public TransmitterArguments (string[] args)
        {
            try
            {

                this.CodeRootFolder = args[0];
                this.ApiBaseUri = new Uri(args[1]);
                this.Repo = args[2];

                if (args.Length == 4)
                {
                    this.RepoStamp = args[3];
                }
                else
                {
                    this.RepoStamp = DateTimeOffset.UtcNow.ToString("O");
                }
            }
            catch (Exception ex)
            {
                Program.Log.Fatal($"Error while reading arguments. Found following {args.Length} args:");
                foreach (string s in args)
                {
                    Program.Log.Fatal(s);
                }
                Program.Log.Fatal(ex);
                throw;
            }
        }

        public string CodeRootFolder { get; set; }
        public string Repo { get; set; }
        public string RepoStamp { get; set; }
        public Uri ApiBaseUri { get; set; }



    }
}