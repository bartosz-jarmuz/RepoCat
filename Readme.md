#  RepoCat ![Image of Yaktocat](https://github.com/bartosz-jarmuz/RepoCat/blob/master/src/RepoCat.Portal/wwwroot/images/cats/cat_laptop64.png)

A searchable catalog of items in a repository.  
Originally developed for software projects and internal tools & scripts used within an enterprise, but there's nothing against using it for aggregating data about other types of items (books, employees).

## Main ideas & goals
 * Minimum overhead of adding items to catalog
 * Maximum flexibility of data presentation
 * Simple search yielding relevant, weight-ordered results

### Also important goals
 * Multiple types of repositories/catalogs compatible
   * GIT/TFS (.NET csprojects)
   * Excel :) (Centralized catalog of anything)
   * Folder structure (Decentralized catalog of anything)
 * Disposable database
   * Repository is refereshed periodically (either on CI pipelines or scheduled background jobs)
 

## Features
 * Multiple repository support
 * Keyword searching in project names, descriptions and tags
 * Regular expression based search
 * Automated extraction of data from within project code
   * Currently .NET only
 
## Screenshots

### Main search page (choose one or more repository)
![Main page](https://raw.githubusercontent.com/bartosz-jarmuz/RepoCat/master/docs/RC_SearchPage.png)

### Repository page (with a search phrase entered)
![Repo page](https://raw.githubusercontent.com/bartosz-jarmuz/RepoCat/master/docs/RC_RepoSearch.png)

### Search filter view (with a columns of repository customized)
![Repo page](https://raw.githubusercontent.com/bartosz-jarmuz/RepoCat/master/docs/RC_FilterResults.png)

 
## Contributions
Very welcome - please create forks and pull requests
