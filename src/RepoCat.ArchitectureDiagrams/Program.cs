using System;
using Structurizr;
using Structurizr.Api;

namespace RepoCat.ArchitectureDiagrams
{
    class Program
    {
        static void Main(string[] args)
        {
            Workspace workspace = new Workspace("RepoCat", "Repository catalog model");
            Model model = workspace.Model;
            ViewSet views = workspace.Views;

            SetupStyles(views);

            model.Enterprise = new Enterprise("Organization which owns some software");

            Person user = model.AddPerson(
                Location.Internal,
                "Employee",
                "Member of any team in the company. Needs info about existing software.");

            SoftwareSystem repoCatSystem = model.AddSoftwareSystem(
                Location.Internal,
                "RepoCat",
                "Repository Catalog\r\n" +
                "Allows browsing the content of repositories");
            user.Uses(repoCatSystem, "Uses");

            SoftwareSystem souceControlSystem = model.AddSoftwareSystem(
                Location.Internal,
                "Repository",
                "Any source control system to which RepoCat has access.");
            souceControlSystem.AddTags(ExistingSystemTag);

            repoCatSystem.Uses(
                souceControlSystem,
                "Gathers info about projects");

            SoftwareSystem telemetrySystem = model.AddSoftwareSystem(
                Location.External,
                "ApplicationInsights",
                "Stores telemetry, allows diagnosing issues");
            telemetrySystem.AddTags(ExistingSystemTag);

            repoCatSystem.Uses(
                telemetrySystem,
                "Sends usage statistics");

            Person developer = model.AddPerson(
                Location.Internal,
                "Developer",
                "Responsible for code, creates and maintains projects");

            developer.Uses(souceControlSystem, "Stores codes, adds manifest files");

            
            Container repoCatPortal = repoCatSystem.AddContainer(
                "Web App",
                "User friendly catalog view",
                "ASP.NET CORE MVC");
            repoCatPortal.AddTags(WebBrowserTag);

            Container database = repoCatSystem.AddContainer(
                "Database", "Stores data",
                "MongoDB");
            database.AddTags(DatabaseTag);

            user.Uses(repoCatPortal, "Uses", "");
            repoCatPortal.Uses(database,
                "CRUD",
                "MongoDB C# Driver");

            repoCatPortal.Uses(souceControlSystem,
                "Uses",
                "JSON/HTTPS");

            Component manifestsController =
                repoCatPortal.AddComponent("ManifestController", "Accepts manifests transmission");

            Component repositoryController =
                repoCatPortal.AddComponent("RepositoryController", "Shows content of a specific repository");

            Component searchController =
                repoCatPortal.AddComponent("SearchController", "Handles searching across various repositories");

            Component databaseFacade =
                repoCatPortal.AddComponent("DatabaseFacade", "Handles CRUD in the database");
            databaseFacade.Uses(database, "Read/Write");
            manifestsController.Uses(databaseFacade, "Stores project manifests");
            repositoryController.Uses(databaseFacade, "Reads project manifests from a given repository");
            searchController.Uses(databaseFacade, "Retrieves projects matching search phrase");


            model.AddImplicitRelationships();


            SystemContextView systemContextView = views.CreateSystemContextView(
                repoCatSystem,
                "SystemContext",
                "RepoCat system model");

            systemContextView.AddAllPeople();
            systemContextView.AddAllElements();

            ContainerView containerView = views.CreateContainerView(
                repoCatSystem,
                "Containers",
                "RepoCat system containers diagram");
            containerView.AddAllPeople();
            containerView.AddAllContainers();
            containerView.Add(souceControlSystem);


            ComponentView componentView =
                views.CreateComponentView(repoCatPortal, "Components", "Components of the RepoCat");
            componentView.AddAllComponents();
            componentView.AddAllContainers();


            StructurizrClient structurizrClient = new StructurizrClient("ed288493-c0dc-4670-9cfb-ab0978e584c3", "6fba2260-de54-4656-a130-debe10fefb16");
            structurizrClient.PutWorkspace(52697, workspace);

        }

        private static void SetupStyles(ViewSet views)
        {
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Container) { Background = "#438dd5", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Component) { Background = "#85bbf0", Color = "#000000" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person, FontSize = 22 });
            styles.Add(new ElementStyle(ExistingSystemTag) { Background = "#999999", Color = "#ffffff" });
            styles.Add(new ElementStyle(DatabaseTag) { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle(WebBrowserTag) { Shape = Shape.WebBrowser });
        }

        public const string ExistingSystemTag = "Existing System";
        public const string DatabaseTag = "Database";
        public const string WebBrowserTag = "WebBrowser";
    }
}
