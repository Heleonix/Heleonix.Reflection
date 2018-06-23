# Heleonix.Reflection

Provides reflection functionality to search and invoke type members

## Install

https://www.nuget.org/packages/Heleonix.Reflection

## Documentation

### Heleonix.Reflection.MembersInfo

Provides information about found members.

#### Properties

* `public Type ContainerType { get; set; }`

  Gets or sets the type of the container in which members (declared in it or in base classes) were found.

* `public object ContainerObject { get; set; }`

  Gets or sets the container object - instance of the type in which members (declared in it or in base classes) were found.

* `public List<MemberInfo> Members { get; }`

  Gets the found members.

### Heleonix.Reflection.Reflector

Provides functionality for working with reflection.

#### Methods

* `public static MembersInfo GetInfo(object instance, Type type, string memberPath, Type[] paramTypes = null, bool requireIntermediateValues = false, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)`

  Gets information about members.

  ##### Parameters

  * `instance`: A root object.

  * `type`: A type of a root object. If `instance` is not `null`, then its typeis used instead.

  * `memberPath`: A path to a member.

  * `paramTypes`: Defines types of parameters to find methods or constructors. If `null` is passed, then types of parameters are ignored.

  * `requireIntermediateValues`: Determines whether intermediate members within the given path must not be `null`.

    ```csharp
    var tuple = new Tuple<Tuple<int>(null);

    var info1 = Reflector.GetInfo(tuple, null, "Item1.Item1", requireIntermediateValues: true);

    // info1 == null

    var info2 = Reflector.GetInfo(tuple, null, "Item1.Item1", requireIntermediateValues: false);

    // info2 == typeof(int)
    ```

  * `bindingFlags`: Defines binding flags to find members.

  ##### Returns

  Information about found members or `null` if no members are found or they are not reachable or they are not accessible.

  ##### Exceptions

  * `AmbiguousMatchException`: More than one member is found on the intermediate path of the `memberPath`.

  * `TargetInvocationException`: An intermediate member on a path threw an exception. See inner exception for details.

  * `InvalidOperationException`: Failed to invoke an intermediate member on a path. See inner exception for details.

  ##### Example

  ```csharp
  var dt = DateTime.Now;

  var info = Reflector.GetInfo(instance: dt, type: null, memberPath: "TimeOfDay.Negate", requireIntermediateValues: true);

  // info.ContainerObject == dt;
  // info.ContainerType == typeof(DateTime);
  // info.Members[0]: MethodInfo about the Negate method
  ```

* `public static bool IsStatic(PropertyInfo info)`

  Determines whether the specified property is static by its getter (if it is defined) or by its setter (if it is defined).

* `public static IList<Type> GetTypes(string simpleName)`

  Gets the types by a simple name (a name without namespace) in the calling assembly and in the assemblies loaded into the current domain.

* `public static string GetMemberPath<TObject>(Expression<Func<TObject, object>> expression)`

  Gets a path to a member which returns some type.

  ##### Example

  ```csharp
  var path = Reflector.GetMemberPath<DateTime>(dt => dt.TimeOfDay.Negate());

  // path: "TimeOfDay.Negate"
  ```

* `public static string GetMemberPath<TObject>(Expression<Action<TObject>> expression)` gets a path to a member which returns `void`.

  ##### Example

  ```csharp
  var path = Reflector.GetMemberPath<List<int>>(list => list.Clear());

  // path: "Clear"
  ```

* `public static string GetMemberPath(Expression expression)`

  Gets a path to a member using the specified (probably dynamically built) expression, which must be of type or inherited from the `LambdaExpression`.

* `public static bool Set(MembersInfo info, object value)`

  Sets a provided value to the provided `MembersInfo`.

  ##### Returns

  `true` in case of success, otherwise `false` if the `info` is `null`
  or `MembersInfo.ContainerObject` is `null` and a member is not static
  or `PropertyInfo.CanWrite` is `false`.

  ##### Exceptions

  * `TargetInvocationException`: Target thrown an exception during execution. See inner exception for details.

  * `InvalidOperationException`: Could not invoke a member for current object's state. See inner exception for details.

* `public static bool Get<TReturn>(MembersInfo info, out TReturn value)`

  Gets a value of the provided `MembersInfo`.

  ##### Returns

  `true` in case of success, otherwise `false` if an `info` is `null`
  or the `MembersInfo.ContainerObject` is `null` and a member is not static
  or the return value is not of type `TReturn`
  `PropertyInfo.CanRead` is `false`.

  ##### Exceptions

  * `TargetInvocationException`: Target thrown an exception during execution. See inner exception for details.

  * `InvalidOperationException`: Could not invoke a member for current object's state. See inner exception for details.

* `public static bool Invoke<TReturn>(MembersInfo info, out TReturn result, params object[] arguments)`

  Invokes a method provided by `info` with specified arguments.

  ##### Returns

  `true` in case of success, otherwise `false` if the `info` is `null`
  or the `MembersInfo.ContainerObject` is `null` and a member is not static
  or the return value is not of type `TReturn`.

  ##### Exceptions

  * `TargetInvocationException`: Target thrown an exception during execution. See inner exception for details.

  * `InvalidOperationException`: Could not invoke a member for current object's state. See inner exception for details.