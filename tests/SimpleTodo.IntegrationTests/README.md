## Problems
If you want to reset database after every test case execution it is problematic in case your
tests run in parallel. You can disable parallel execution but time of execution may hurt you.

You can not to clear database, you have to pay attention in assertions especially in case of search queries.

## xUnit

#### Test setup and tear down
xUnit do not have explicit methods like in nUnit.
For every test case new instance of test class is created.
So you treat constructor as setup so every object instantiated in constructor is created for every test case.
For context cleanup, add the IDisposable interface to your test class, and put the cleanup code in the Dispose() method.

#### Parallelism
In xUnit parallelism is enabled by default.
Every test class is treated as a collection.
Collections run in parallel.
Test cases in collection do not run in parallel.
So rule of thumb may be to make test collections (classes) small to shorten the time of tests execution.

You could disable all parallelism in your test assembly by setting the `parallelizeTestCollections` configuration 
setting to `false`.

### xUnit shared state

### State shared for single test class
To share the same dependencies between test cases you can use `IClassFixture<T>` interface.
So it allows you to share object instance across tests in a single class (class with test cases).

For every such class being shared for test class you have to create separate fixture class so test class can implement more
`IClassFixture<T>` interfaces.  Note that you cannot control the order that fixture objects are created, and fixtures 
cannot take dependencies on other fixtures. If you have need to control creation order and/or have dependencies 
between fixtures, you should create a class which encapsulates the other two fixtures, so that it can do the 
object creation itself.

To treat two or more test classes as one collection you can use `CollectionDefinition` attribute.
To categorize all the tests classes under the same collection use `CollectionDefinition` attribute
on class implementing `ICollectionFixture<T>` interface.
xUnit treats collection fixtures the same way as it does class fixtures but their lifetime is longer.
It is created before any tests are run in our test classes in the collection, and will not be cleaned up until all
test classes in the collection have finished running.
So we can call this "test context".

Assembly fixtures can be used to shared object instances across the entire test assembly.
Unlike collection fixtures, there is no change in parallelization when using an assembly fixture.
This means fixtures used as assembly fixtures may be used from multiple tests simultaneously, and must be designed 
for with this parallelism requirement in mind.

Sources:
- https://xunit.net/docs/shared-context
