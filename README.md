# ApiSolutionWolfpack
The solution includes 3 entities - Packs, Wolves and their relation WolfPacks.

They are managed and stored in a inmemory database. Such is used instead of sqlserver for demonstrative purposes.

It is assumed that wolves are identified by ids. Attributes are the name saved as a string, gender saved as an int (by ISO standart), birthdate as a datetime and location of a Wolf is represent by a PointF structure - the x coordinate representing latitude and the y representing longitude. 
The packs are identified by names.
The relation wolfpacks has an id, a packname and a wolfname, representing a many to many relationship between Wolf and Pack.

All tests of the endpoints are conducted with Postman with SSL verification turned off. 

## Wolves

POST: api/Wolves - adds a Wolf object to the Wolves table

sample request body:

        {	    
            "wolfid":1307436,
            "name": "Konstantin",
            "gender": 1,
            "birthdate":"1999-03-24",
            "location": {"x":51.449409,"y":5.494774}
        }

GET: api/Wolves - returns all added wolves and their basic information
  
GET: api/Wolves/id - returns wolf by id with detailed information

PUT: api/Wolves/id - updates a wolf by id

DELETE: api/Wolves/id - deletes a wolf by id. If the wolf was part of a pack - it is deleted from the pack. If it was the last wolf of the pack - the pack is deleted as well.

GET: api/Wolves/id/location - returns the location of a wolf by id

PATCH: api/Wolves/id/location - updates the location of a wolf by id

sample request body:

        {
          "x":1.2,
          "y":3.4567
        }
        
## WolfPacks

POST: api/WolfPacks  - adds a relation between wolf and a pack. If it is a new pack - it adds it to the packs table.

sample request body:

        {
          "wolfid":1307436,
          "packname":"Alpha"
        }

GET: api/WolfPacks - returns packs with their respective wolves.

GET: api/WolfPacks/packname - returns the wolves in a pack by packname

GET: api/WolfPacks/packname/id - returns information about a wolf by id in a pack by packname

DELETE: api/WolfPacks/packname - deletes a pack by packname. Deletes all relations with the pack and the wolves and deletes the pack from table packs.

DELETE: api/WolfPacks/packname/id - deletes a wolf by id from a pack by packname. If it was the last wolf, it deletes the pack.

## Packs

GET: api/Packs - returns a list of packs

GET: api/Packs/packname - returns the name of one pack by packname

DELETE: api/Packs/packname - deletes a pack by packname. All relations of wolves with the pack are also deleted.
