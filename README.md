# taxonomy-api

GraphQL service to retrieve Taxonomy Information

## Example Requests


Get the index taxonomy with the first level of children

```
{
    index {
        id,
        nodeType,
        fullStatement,
        children {
            id,
            fullStatement,
            nodeType
        }
    }
}
```

Get a specific taxonomy (without children), N.B. id is mandatory on a taxonomy query

```
{
    taxonomy(id: "269bdc7c-aff1-4bf2-b210-2e07a5c726aa") {
        id,
        nodeType,
        fullStatement
    }
}
```

Get a specific taxonomy filtering returned chidren N.B. does nothing if children not requested, and will apply to all levels of children requested
```
{
    taxonomy(id: "269bdc7c-aff1-4bf2-b210-2e07a5c726aa", nodeType:"Grade Level") {
        id,
        nodeType,
        fullStatement,
        children {
            id,
            fullStatement,
            nodeType
        }
    }
}
```

Similar to the above, to request a taxonomy that is linked to a taxonomy (but isn't a child), do the following giving the id of the parent node.  
Note this returns an array 
```
{
    linked_taxonomy(id: "269bdc7c-aff1-4bf2-b210-2e07a5c726aa") {
        id,
        nodeType,
        fullStatement,
        children {
            id,
            fullStatement,
            nodeType
        }
    }
}
```

This app does cache taxonomies and you can request a child element from a taxonomy document, however, once the cache expires the app will not be easily able to query for the tree from that level again.

In order to be able to fetch again the parent document ID will need to be supplied, therefore you should ensure the documentId is passed when trying to fetch individual nodes.  
Alternatively, load the entire doc into your own app and handle there
```
{
    taxonomy(documentId: "269bdc7c-aff1-4bf2-b210-2e07a5c726aa", id: "7d80eac7-918b-11ed-aefa-022584f5db21") {
        id,
        children {
            id,
            fullStatement,
            nodeType
        }
    }
}
```

Example with multiple child levels and nodeType filter can take an array of nodes you want to see
```
{
    taxonomy(id: "b7e792e4-a2b7-4bc5-8ff9-d5a5e47826d8", nodeType: ["Grade Level","Subjects"]) {
        id,
        nodeType,
        fullStatement,
        children {
            id,
            fullStatement,
            nodeType,
            children {
                id,
                fullStatement,
                nodeType
            }
        }
    }
}
```

## Example response

As per the last request above, you would get the following back

```
{
    "data": {
        "taxonomy": {
            "id": "b7e792e4-a2b7-4bc5-8ff9-d5a5e47826d8",
            "nodeType": "Document",
            "fullStatement": "REFERENCE A Taxonomy Index",
            "children": [
                {
                    "id": "e13a8479-1e4e-42cd-ad54-8261430872de",
                    "fullStatement": "G2/Y3",
                    "nodeType": "Grade Level",
                    "children": [
                        {
                            "id": "27e20e6c-6def-4771-a372-2a9f7f14a07f",
                            "fullStatement": "Science",
                            "nodeType": "Subjects"
                        },
                        {
                            "id": "2e419e6a-e29a-435c-9aa3-4540e2aa09e5",
                            "fullStatement": "Maths",
                            "nodeType": "Subjects"
                        },
                        {
                            "id": "26708bc8-56a2-4c8e-9faa-5290e0723549",
                            "fullStatement": "English",
                            "nodeType": "Subjects"
                        }
                    ]
                },
                {
                    "id": "269bdc7c-aff1-4bf2-b210-2e07a5c726aa",
                    "fullStatement": "G3/Y4",
                    "nodeType": "Grade Level",
                    "children": []
                }
            ]
        }
    }
}
```