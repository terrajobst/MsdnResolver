# MSDN Resolver

This simple console app shows how you can get an MSDN URL, such as
<http://msdn.microsoft.com/en-us/library/6sh2ey19>, from an API,
such as ``T:System.Collections.Generic.List`1``:

```C#
// The documentation IDs are the same that the C#
// uses in the documentation XML files.
var documentationId = "T:System.Collections.Generic.List`1";

var finder = new MsdnUrlFinder();
var url = await finder.GetUrlAsync(documentationId);

// Prints
// http://msdn.microsoft.com/en-us/library/6sh2ey19
Console.WriteLine(url);
```