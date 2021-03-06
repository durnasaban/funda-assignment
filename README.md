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
  - It uses a staging table for storing objects which come from funda API.
  - Staging table is deleted before starting to consume the API
  - The objects are pulled from the api and stored in Mongo Db page by page.
  - Top agents are calculated and pulled from Mongo Db and push them with the key, which is determined in appsettings, into Redis cache.
- Details
  - The workflow is run for every parameter which determines in appsettings
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
  - The key of funda API is stored in secret.json to be able to avoid observing it on the online repository.
  - The execution period of the worker service can be set in appsettings with this parameter: `ExecutionPeriodInMinutes`

### [Top Agents API](https://github.com/durnasaban/funda-assignment/tree/main/src/APIs/TopAgentsApi)
- Worflow
  - The key is gotten from the API request and the top agent values are gotten from the Redis cache with that key. For Example: `TopAgentsInAmsterdam`, `TopAgentsInAmsterdamWithGarden`
  - The key should be one of the keys which are determined in appsettings in Worker Service.
  - Sample api url is `http://localhost:5000/api/top-agents/TopAgentsInAmsterdam`  
- Details
  - Used [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit) library for setting limits to the requests.
  - Added a custom middleware for catching exceptions and returning 500 status code with a meaningful message.
  
### [UI](https://github.com/durnasaban/funda-assignment/tree/main/src/UI)
- Workflow
  - Fetch function in Javascript consumes top agents API with 2 parameters. `TopAgentsInAmsterdam`, `TopAgentsInAmsterdamWithGarden`
  - The responses are loaded into the particular tables on the HTML page.
- Details
  - Used an HTML page along with a javascript file instead of the js frameworks such as react js, vue js, etc. because of the suggestions in the assignment doc.
  - Bootstrap is used for better view of tables.
  - index.html page should be opened in a browser for being able to see the top agents.
