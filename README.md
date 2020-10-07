# MD5
A C# implementation of the MD5 hashing algorithm, originally written in 2018. Since [MD5 is vulnerable to collision attacks](https://www.mscs.dal.ca/~selinger/md5collision/), it should no longer be used as a cryptographic hash function, but it can still be used for file integrity checks.

This implementation was written as part of a learning process to better understand the algorithm. For any important tasks, please use a mature implementation of the MD5 algorithm.

The implementation can display the results of each step of the hashing process to the user if the -v argument is applied.




## Arguments
|Parameter       |  Default  | Description                                                                                             |
|----------------|-----------|---------------------------------------------------------------------------------------------------------|
|-v              | false     | (Verbose) Print the results of every step of the hashing process                                        |
|-x              | false     | Run the test suite                                                                                      |
|-i              | null      | Used to specify a string to hash                                                                        |
|-f              | null      | Used to specify a file to hash                                                                          |