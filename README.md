# Heleonix.Reflection

[![Release: .NET / NuGet](https://github.com/Heleonix/Heleonix.Reflection/actions/workflows/release-net-nuget.yml/badge.svg)](https://github.com/Heleonix/Heleonix.Reflection/actions/workflows/release-net-nuget.yml)

Provides reflection functionality to search and invoke type members, search types, generate delegates etc.

## Install

https://www.nuget.org/packages/Heleonix.Reflection

## Documentation

See [Heleonix.Reflection](https://heleonix.github.io/docs/Heleonix.Reflection)

## Examples

```csharp
var dt = DateTime.Now;

var info = Reflector.GetInfo(instance: dt, type: null, memberPath: "TimeOfDay.Negate");

// info[0].Name == "Negate";
// info[0].MemberType == MemberTypes.Property;
```

```csharp
var path = Reflector.GetMemberPath<DateTime>(dt => dt.TimeOfDay.Negate());

// path: "TimeOfDay.Negate"
```

```csharp
var success = Reflector.Get(DateTime.Now, null, "TimeOfDay.Hours", out int value);

// success == true;
// value == DateTime.Now.TimeOfDay.Hours;

or

var success = Reflector.Get(typeof(int), null, "CustomAttributes[0].AttributeType", out int value);

// success == true;
// value == typeof(int).CustomAttributes.First().AttributeType;

or

var success = Reflector.Get(typeof(int), null, "CustomAttributes[0]", out int value);

// success == true;
// value == typeof(int).CustomAttributes.First();
```

```csharp
public class Root
{
    public Child Child { get; set; } = new Child();
    public Child[] Children { get; set; } = new Child[] { new Child(), new Child() };
}

public class Child { public int Value { get; set; } }

var root = new Root();

var success1 = Reflector.Set(root, null, "Child.Value", 111);
var success2 = Reflector.Set(root, null, "Children[0].Value", 222);
var success3 = Reflector.Set(root, null, "Children[1]", new Child() { Value = 333 });

// success1 == true;
// success2 == true;
// success3 == true;

// root.Child.Value == 111;
// root.Children[0].Value == 222;
// root.Children[1].Value == 333;
```

```csharp
var success = Reflector.Invoke(DateTime.Now, null, "Date.AddYears", new[] { typeof(int) }, out DateTime result, arguments: 10);

// success == true;
// result.Year == DateTime.Now.Date.Year + 10;
```

```csharp
var getter = Reflector.CreateGetter(dt => dt.Date.Month);

var value = getter(DateTime.Now);

// value == DateTime.Now.Date.Month;
```

```csharp
public class Root { public Child Child { get; set; } = new Child(); }
public class Child { public int Value { get; set; } }

var setter = Reflector.CreateSetter{Root, int}(r => r.Child.Value);
var root = new Root();

setter(root, 12345);

// root.Child.Value == 12345;
```

```csharp
public class Root { public Child Child { get; set; } = new Child(); }
public class Child { public int Value { get; set; } }

var setter = Reflector.CreateSetter{Root, int}("Child.Value", typeof(Root));
var root = new Root();

setter(root, 12345);

// root.Child.Value == 12345;
```

## Contribution Guideline

1. [Create a fork](https://github.com/Heleonix/Heleonix.Reflection/fork) from the main repository
2. Implement whatever is needed
3. [Create a Pull Request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-a-pull-request-from-a-fork).
   Make sure the assigned [Checks](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/collaborating-on-repositories-with-code-quality-features/about-status-checks#checks) pass successfully.
   You can watch the progress in the [PR: .NET](https://github.com/Heleonix/Heleonix.Reflection/actions/workflows/pr-net.yml) GitHub workflows
4. [Request review](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/requesting-a-pull-request-review) from the code owner
5. Once approved, merge your Pull Request via [Squash and merge](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/incorporating-changes-from-a-pull-request/about-pull-request-merges#squash-and-merge-your-commits)

   > **IMPORTANT**  
   > While merging, enter a [Conventional Commits](https://www.conventionalcommits.org/) commit message.
   > This commit message will be used in automatically generated [Github Release Notes](https://github.com/Heleonix/Heleonix.Reflection/releases)
   > and [NuGet Release Notes](https://www.nuget.org/packages/Heleonix.Reflection/#releasenotes-body-tab)

6. Monitor the [Release: .NET / NuGet](https://github.com/Heleonix/Heleonix.Reflection/actions/workflows/release-net-nuget.yml)
   GitHub workflow to make sure your changes are delivered successfully
7. In case of any issues, please contact [heleonix.sln@gmail.com](mailto:heleonix.sln@gmail.com)
