
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

In addition, the idea

## Features

- Dynamic database to store arbitrary JSON objects
- Quite optimized, as it doesn't have to serialize/deserialize
    except when explicitly retrieving data, instead using JSON flattening
    as described in the **How it works** section.
- Compatibility with Firebase REST API

## Firebase API Compatibility

**dynDB-x** implements the Firebase REST API, based on the documentation.
It even uses the same algorithm for push IDs, so data will be sorted
chronologically just like in Firebase.

## License

Copyright &copy; 2016 0xFireball. All Rights Reserved.

Licensed under the AGPLv3.