# Funda Assignment

## 1. Requirements
- List top 10 real estate agents which have most sale objects in Amsterdam
- List top 10 real estate agents which have most sale objects with garden in Amsterdam
- If there are too many API requests in a short period of time (&gt;100 per minute), the API will reject the requests.

## 2. Architecture

![Funda Assignment Architecture (1)](https://user-images.githubusercontent.com/25878234/115166115-b61de480-a0b1-11eb-9b61-cb45901a9bfb.png)

## 3. Project Details

### [Caching Objects Worker Service](https://github.com/durnasaban/funda-assignment/tree/main/src/CachingObjects)
- Workflow
  - It uses staging table for storing objects which come from funda api.
  - Staging table is deleted before starting to consume the api
  - The objects are pulled from the api and stored in Mongo Db page by page.
  - Top agents are calculated and pulled from Mongo Db and push these information with the key, which is set in appsettings, into Redis cache.
- Details
  - The workflow is run for every parameters which determines in appsettings
    ````
    {
        "Key": "TopAgentsInAmsterdam",
        "SearchQuery": "amsterdam",
        "TopAgentCount": 10
    },
    {
        "Key": "TopAgentsInAmsterdamWithGarden",
        "SearchQuery": "amsterdam/tuin",
        "TopAgentCount": 10
    }
    ````
  - The key of funda api stores in secret.json to be able to avoid observe it on the online repository.
  - Execution period of the worker service can be set in appsettings with this parameter: `ExecutionPeriodInMinutes`

### [Top Agents API](https://github.com/durnasaban/funda-assignment/tree/main/src/APIs/TopAgentsApi)
- Worflow
  - The key is gotten from the api request and the top agent values are gotten from the redis cache with that key. For Example: `TopAgentsInAmsterdam`, `TopAgentsInAmsterdamWithGarden`
  - The key should be one of the keys which determine in appsettings in WorkerService.
  - Sample api url is `http://localhost:5000/api/top-agents/TopAgentsInAmsterdam`  
- Details
  - Used [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit) library for being able to set request limits to the clients.
  - Add a custom middleware for cacthing exceptions and returns 500 status code with a meaningful message.
  
### [UI](https://github.com/durnasaban/funda-assignment/tree/main/src/UI)
- Workflow
  - Fetch function in Javascript consumes top agents api with 2 parameters. `TopAgentsInAmsterdam`, `TopAgentsInAmsterdamWithGarden`
  - Load the responses to the particular tables
- Details
  - Used an html page along with a javascript file instead of the js frameworks such as react js, vue js etc. because of the suggestions in the assignment doc.
  - Bootstrap is used for better view of tables.
  - index.html page should be opened in a browser for being able to see the top agents.
