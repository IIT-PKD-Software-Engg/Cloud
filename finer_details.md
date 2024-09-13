## TODO:
1. Expand the Cloud, Storage and Database classes
2. Verify the data types of everything

## Some Explanation:
* Whiteboard team - they should only be able to access the whiteboard folder for their storage
* Content team - they should only be able to access the content folder for their storage
How to do so?

Class Details:
1. WhiteBoardCloudProxy is for the access of the WhiteBoard Team
2. ContentCloudProxy is for the access of the Content Team
3. Cloud is the general implementation of the system, irrespective of the type of storage mechanism used.
4. ICloud is the interface used with other classes
5. Storage is the proper implementation of the file storage
6. Database is the index file of the storage system

* We will have the actual storage class, and a database class

* IStorage and IDatabase will be given by the cloud provider

How are we storing the data:
1. string dataUri
2. object data
3. int timestamp

## Class Diagram
![cloud](Class Diagram)


# Add App Configuration files for rest of the module which will be called first
# Add User Configuration files if possible

# add encryption to chats

# look into storage of large files

# prepare cloud ux