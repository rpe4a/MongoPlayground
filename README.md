# MongoPlayground

### Start mongo in Docker
docker run --name mongodb -d -v %YOUR_LOCAL_DIR%:/data/db -p 27017:27017 mongo

### Aggregation example:
```js
db.zipcodes.aggregate([
    {$match:{pop:{$gte:50000}}},
    {$project:{_id:0, city:{$toLower:"$city"}, pop:1, state:1, zip:"$_id"}},
    {$group : {_id : {"city_name" : "$city"}, "average_pop" : {$avg : "$pop"}}},
    {$sort: {"_id.city_name" : 1}},
    {$skip: 10},
    {$limit: 5}
])

db.zipcodes.aggregate([
    {$project:
            {
                "city" : "$city",
                "population" : "$pop",
                "first_char": {$substr : ["$city",0,1]},
            }
    },
    {$match:
            {
                "first_char" : {$in : ["A", "B", "C"]}
            }
    },
    {$group:
            {
                "_id" : null,
                "sum_population" : {$sum: "$population"}
            }
    }
])
```

### Query plan explanation 
`db.zipcodes.explain().find({"state" : "CA"})`

### Indexes
`db.zipcodes.createIndex({"state": 1}, {"name": "state-asc"})`

### Full text search
```js
db.texts.createIndex({"news" : "text"})
db.texts.find({$text: {$search: "hope"}})
```