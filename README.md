
# dynDB-x


## About

**dynDB-x** is an experiment I put together that demonstrates storing
JSON-like data in a NoSQL or other key-value database like Redis.

The server provides a RESTful API to access the database. The API
is modeled after and fully compatible with the [Firebase REST API](https://firebase.google.com/docs/reference/rest/database/).
Even the Firebase push IDs are generated with the same algorithm.
Refer to the **Firebase API Compatibility** section for more information.

This particular implementation is designed for use with LiteDB,
but almost all of the code can be directly ported to work on any other
key-value database.

Inspiration for this project came from this article discussing
how to effectively store JS objects in Redis:
<https://medium.com/@stockholmux/store-javascript-objects-in-redis-with-node-js-the-right-way-1e2e89dbbf64#.okt3eevtc>

In addition, the abilities `dynDB-x` are heavily influenced by
those of the Firebase Database and its API.

## Features

- Dynamic database to store arbitrary JSON objects
- Reasonably fast, as it doesn't have to serialize/deserialize
    except when explicitly retrieving data, instead using JSON flattening
    as described in the **How it works** section.
- Compatibility with Firebase REST API

## How it works

### Quick intro to bad solution

The obvious way to build a JSON-backed database that is
backed by a key-value store would be to store serialized objects
in the database, then deserialize them when they are needed.
The modified objects would then be serialized again and stored.

However, this method does not scale at all, as when the
amount of data grows, the time for these operations increases
significantly. In addition, memory usage will quickly go out
of hand with large databases.

### Better solution

A better, but still not ideal solution would be the one
that this project implements. See the section below for areas
to improve that I have identified for now. This approach
would be to flatten the JSON object and store a hashtable
representing this data. For example, see the following JSON object
and its flattened representation:

JSON string:

```json
{
    "name": "Bob",
    "pets": [
        "dog",
        "cat"
    ],
    "info": {
        "age": 30,
        "secrets": [
            9999
        ]
    }
}
```

Flattened representation (hashtable):

```text
name: "Bob"
pets[0]: "dog"
pets[1]: "cat"
info.age: 30
info.secrets[0]: 9999
```

So now, instead of serializing/deserializing, we just store
an empty hashtable, and merge/overwrite it with new data! This
is far more efficient, as you can perform most operations on the
flattened representation without unflattening and deserializing
the data!

For example, to insert an object, you would just need to create
a new flattened hashtable representation of the new data with the insertion
path prepended to the flattened key. Then you could merge that hashtable with
the root object, and now you have inserted an object!

To delete a node, you simply find keys with a matching prefix and remove
them from the hashtable! All these operations would normally be done
on a deserialized representation.

The only time the data needs to be deserialized is when the user queries
for raw JSON!

All the code used for flattened JSON object manipulation is encapsulated
into a reusable .NET Core library! See `dynDBx\src\IridiumIon.JsonFlat2`.
All code in this project is licensed under the AGPLv3.

### Much better solution (improvements!)

The previously described is definitely not the best solution, though it is relatively
easy to implement. The problems with this approach are:

- Memory usage for the hashtable storing values is still high;
    when the flattened JSON object is retrieved, the entire
    mapping is loaded into memory. While this will almost certainly
    use less memory than deserializing the entire JSON string, it isn't
    optimal either.
    - This isn't too hard to fix; instead of loading the entire
        hashtable into memory, parts of it can be loaded as needed.
        However, this will result in a small performance decrease.

I will soon improve this project by addressing these areas of
improvement and making a more performant implementation.

## Firebase API Compatibility

**dynDB-x** implements the Firebase REST API, based on the documentation.
It even uses the same algorithm for push IDs, so data will be sorted
chronologically just like in Firebase.

**However**, **dynDB-x** does not yet support all the permissions
and access rules features of the Firebase API, and is not sufficiently
secure for production use as the data is world readble and writable.
These features may be implemented at a later time.

## License

Copyright &copy; 2016 0xFireball. All Rights Reserved.

Licensed under the AGPLv3.