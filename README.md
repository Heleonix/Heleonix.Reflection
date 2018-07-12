# Heleonix.Reflection

Provides reflection functionality to search and invoke type members, search types, generate delegates etc.

## Install

https://www.nuget.org/packages/Heleonix.Reflection

## API

### Heleonix.Reflection.Reflector

Provides functionality for working with reflection.

#### Methods

* `public static MemberInfo[] GetInfo(object instance, Type type, string memberPath, Type[] parameterTypes = null, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)`

  Gets information about members.

  ##### Parameters

  - `instance`: A root object.

  - `type`: A type of a root object. If `instance` is not `null`, then its type is used instead.

  - `memberPath`: A path to a member.

  - `parameterTypes`: Types of parameters to find methods or constructors. If `null` is passed, then types of parameters are ignored.

  - `bindingFlags`: Defines binding flags to find members.

  ##### Exceptions

  - `TargetException`: An intermediate member on a path threw an exception. See inner exception for details.

  ##### Returns

  Information about found members or an empty array if no members are found or they are not reachable or they are not accessible.

  ##### Example

  ```csharp
  var dt = DateTime.Now;

  var info = Reflector.GetInfo(instance: dt, type: null, memberPath: "TimeOfDay.Negate");

  // info[0].Name == "Negate";
  // info[0].MemberType == MemberTypes.Property;
  ```

* `public static bool IsStatic(PropertyInfo info)`

  Determines whether the specified property is static by its getter (if it is defined) or by its setter (if it is defined).

* `public static Type[] GetTypes(string simpleName)`

  Gets the types by a simple name (a name without namespace) in the calling assembly and in the assemblies loaded into the current domain.

* `public static string GetMemberPath<TObject>(Expression<Func<TObject, object>> memberPath)`

  Gets a path to a member which returns some type.

  ##### Example

  ```csharp
  var path = Reflector.GetMemberPath<DateTime>(dt => dt.TimeOfDay.Negate());

  // path: "TimeOfDay.Negate"
  ```

* `public static string GetMemberPath<TObject>(Expression<Action<TObject>> memberPath)`

  Gets a path to a member which returns `void`.

  ##### Example

  ```csharp
  var path = Reflector.GetMemberPath<List<int>>(list => list.Clear());

  // path: "Clear"
  ```

* `public static string GetMemberPath(LambdaExpression memberPath)`

  Gets a path to a member using the specified (probably dynamically built) expression.

  ##### Returns

  A name of a member or an empty string if `expression` is `null`.

* `public static bool Get<TReturn>(object instance, Type type, string memberPath, out TReturn value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)`

  Gets a value by the provided path.

  ##### Parameters

  - `instance`: A root object.

  - `type`: A type of a root object. If `instance` is not `null`, then its type is used instead.

  - `memberPath`: A path to a member.

  - `value`: A gotten value.

  - `bindingFlags`: Binding flags to find members.

  ##### Exceptions

  - `TargetException`: Target thrown an exception during execution. See inner exception for details.

  ##### Returns

  `true` in case of success, otherwise `false` if `memberPath` is `null` or empty
  or
  `instance` is `null` and `type` is `null`
  or
  a target member or one of intermediate members was not found
  or
  a member is not static and its container is null
  or
  a target member or an intermediate member is neither `PropertyInfo` nor `FieldInfo`
  or
  a target value is not of type `TReturn`.

  ##### Example

  ```csharp
  var success = Reflector.Get(DateTime.Now, null, "TimeOfDay.Hours", out int value);

  // success == true;
  // value == DateTime.Now.TimeOfDay.Hours;
  ```

* `public static bool Set(object instance, Type type, string memberPath, object value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)`

  Sets a provided value by the provided path.

  ##### Parameters

  - `instance`: A root object.

  - `type`: A type of a root object. If `instance` is not `null`, then its type is used instead.

  - `memberPath`: A path to a member.

  - `value`: A value to be set.

  - `bindingFlags`: Binding flags to find members.

  ##### Exceptions

  - `TargetException`: Target thrown an exception during execution. See inner exception for details.

  ##### Returns

  `true` in case of success, otherwise `false` if `memberPath` is `null` or empty
  or
  `instance` is `null` and `type` is `null`
  or
  a target member or one of intermediate members was not found
  or
  a member is not static and its container is null
  or
  a target member or an intermediate member is neither `PropertyInfo` nor `FieldInfo`.

  ##### Example

  ```csharp
  public class Root { public Child Child { get; set; } = new Child(); }
  public class Child { public int Value { get; set; } }

  var root = new Root();
  var success = Reflector.Set(root, null, "Child.Value", 12345);

  // success == true;
  // root.Child.Value == 12345;
  ```

* `public static bool Invoke<TReturn>(object instance, Type type, string memberPath, Type[] parameterTypes, out TReturn returnValue, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, params object[] arguments)`

  Invokes a method or constructor by the provided path. Use "ctor" to invoke constructors, i.e."Item.SubItem.ctor".

  ##### Parameters

  - `instance`: A root object.

  - `type`: A type of a root object.

  - `instance`: is not `null`, then its runtime type is used instead.

  - `memberPath`: A path to a member to invoke.

  - `parameterTypes`: Types of parameters to find a method by. Pass `null` to ignore parameters, or an empty array for parameterless methods.

  - `returnValue`: A value to be returned if a member is not void.

  - `bindingFlags`: Binding flags to find members.

  - `arguments`: Arguments to be passed into a member to invoke.

  ##### Exceptions

  - `TargetException`: Target thrown an exception during execution. See inner exception for details.

  ##### Returns

  `true` in case of success, otherwise `false` if `memberPath` is `null` or empty
  or
  `instance` is `null` and `type` is `null`
  or
  a target member or one of intermediate members was not found
  or
  an intermediate member is neither `PropertyInfo` nor `FieldInfo`
  or
  an intermediate member is not static and its container is null
  or
  a target member is not `MethodBase`
  or
  a target value is not of type `TReturn`.

  ##### Example

  ```csharp
  var success = Reflector.Invoke(DateTime.Now, null, "Date.AddYears", new[] { typeof(int) }, out DateTime result, arguments: 10);

  // success == true;
  // result.Year == DateTime.Now.Date.Year + 10;
  ```

* `public static Func<TObject, TReturn> CreateGetter<TObject, TReturn>(Expression<Func<TObject, TReturn>> memberPath)`

  Creates a getter. Works with exactly specified types without conversion. This is the fastest implementation.

  ##### Parameters

  - `memberPath`: The path to a member.

  ##### Returns

  A compiled delegate to get a value or `null` if the `memberPath` is `null`.

  ##### Example

  ```csharp
  var getter = Reflector.CreateGetter(dt => dt.Date.Month);

  var value = getter(DateTime.Now);

  // value == DateTime.Now.Date.Month;
  ```

* `public static Func<TObject, TReturn> CreateGetter<TObject, TReturn>(string memberPath, Type containerType = null)`

  Creates a getter. Can create getters with any convertable types for polimorphic usage.

  ##### Parameters

  - `memberPath`: The path to a member.

  - `containerType`: A type of a container's object which contains the member. If null is specified, then `TObject` is used without conversion.

  ##### Returns

  A compiled delegate to get a value or `null` if the `memberPath` is `null` or empty.

  ##### Example

  ```csharp
  var getter = Reflector.CreateGetter{object, object}("Date.Month", typeof(DateTime));

  var value = getter(DateTime.Now);

  // value == DateTime.Now.Date.Month;
  ```

* `public static Action<TObject, TValue> CreateSetter<TObject, TValue>(Expression<Func<TObject, TValue>> memberPath)`

  Creates the setter. Works with exactly specified types without conversion. This is the fastest implementation.

  ##### Parameters

  - `memberPath`: The path to a member.

  ##### Returns

  A compiled delegate to set a value or `null` if `memberPath` is `null`.

  ##### Example

  ```csharp
  public class Root { public Child Child { get; set; } = new Child(); }
  public class Child { public int Value { get; set; } }

  var setter = Reflector.CreateSetter{Root, int}(r => r.Child.Value);
  var root = new Root();

  setter(root, 12345);

  // root.Child.Value == 12345;
  ```

* `public static Action<TObject, TValue> CreateSetter<TObject, TValue>(string memberPath, Type containerType = null)`

  Creates a setter. Can create setters with any convertable types for polimorphic usage.

  ##### Parameters

  - `memberPath`: The path to a member.

  - `containerType`: A type of a container's object which contains the member. If null is specified, then `TObject` is used without conversion.

  ##### Returns

  A compiled delegate to set a value or `null` if the `memberPath` is `null` or empty.

  ##### Example

  ```csharp
  public class Root { public Child Child { get; set; } = new Child(); }
  public class Child { public int Value { get; set; } }

  var setter = Reflector.CreateSetter{Root, int}("Child.Value", typeof(Root));
  var root = new Root();

  setter(root, 12345);

  // root.Child.Value == 12345;
  ```
