# RepoCat

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
 
 
 
## Contributions
Very welcome - please create forks and pull requests
