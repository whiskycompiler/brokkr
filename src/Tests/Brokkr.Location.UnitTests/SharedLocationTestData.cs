namespace Brokkr.Location.UnitTests;

public record TestRecord(
    string LocationString,
    Type ExpectedLocationType);

public static class SharedLocationTestData
{
    public static readonly TestRecord[] IndeterminateRelativePaths =
    [
        new(".", typeof(IndeterminateRelativePath)),
        new("..", typeof(IndeterminateRelativePath)),
        new("bar", typeof(IndeterminateRelativePath)),
        new("bar.txt", typeof(IndeterminateRelativePath)),
        new(".config", typeof(IndeterminateRelativePath)),
    ];

    public static readonly TestRecord[] RelativeUnixPaths =
    [
        new("./", typeof(RelativeUnixPath)),
        new("./test.nmd", typeof(RelativeUnixPath)),
        new("../../foo.txt", typeof(RelativeUnixPath)),
        new("../../foo bar.txt", typeof(RelativeUnixPath)),
        new("../../foo_(1).png", typeof(RelativeUnixPath)),
        new("../test/foo.md", typeof(RelativeUnixPath)),
        new("../test=/foo.md", typeof(RelativeUnixPath)),
        new("abc/foo/", typeof(RelativeUnixPath)),
        new("abc foo/", typeof(RelativeUnixPath)),
        new("abc/test.md", typeof(RelativeUnixPath)),
        new("abc/.md", typeof(RelativeUnixPath)),
        new("abc/.config/test.md", typeof(RelativeUnixPath)),
        new(".config/test.md", typeof(RelativeUnixPath)),
    ];

    public static readonly TestRecord[] InvalidRelativeUnixPaths =
    [
        // empty directory names
        new("foo//something.md", typeof(void)),
        new(".//", typeof(void)),
    ];

    public static readonly TestRecord[] AbsoluteUnixPaths =
    [
        new("/foo/bar/something.md", typeof(AbsoluteUnixPath)),
        new("/foo/bar/some thing.md", typeof(AbsoluteUnixPath)),
        new("/foo bar/some thing.md", typeof(AbsoluteUnixPath)),
        new("/foo bar/some (1).png", typeof(AbsoluteUnixPath)),
        new("/foo=bar/some.png", typeof(AbsoluteUnixPath)),
        new("/foo/bar/", typeof(AbsoluteUnixPath)),
        new("/foo/bar", typeof(AbsoluteUnixPath)),
        new("/foo/.config/bar", typeof(AbsoluteUnixPath)),
        new("/foo/.config/bar.mp4", typeof(AbsoluteUnixPath)),
        new("/foo/.dat", typeof(AbsoluteUnixPath)),
        new("/", typeof(AbsoluteUnixPath)),
    ];

    public static readonly TestRecord[] InvalidAbsoluteUnixPaths =
    [
        // empty directory names
        new("/foo//something.md", typeof(void)),
        new("//", typeof(void)),
    ];

    public static readonly TestRecord[] AbsoluteWindowsPaths =
    [
        new("C:\\", typeof(AbsoluteWindowsPath)),
        new("D:\\foo\\", typeof(AbsoluteWindowsPath)),
        new("D:\\foo\\bar.txt", typeof(AbsoluteWindowsPath)),
        new("f:\\text.dat", typeof(AbsoluteWindowsPath)),
    ];

    public static readonly TestRecord[] RelativeWindowsPaths =
    [
        new(".\\", typeof(RelativeWindowsPath)),
        new(".\\foo.txt", typeof(RelativeWindowsPath)),
        new("..\\", typeof(RelativeWindowsPath)),
        new("..\\..\\foo.bar", typeof(RelativeWindowsPath)),
        new("abc\\foo\\", typeof(RelativeWindowsPath)),
        new("abc\\test.md", typeof(RelativeWindowsPath)),
    ];

    public static readonly TestRecord[] Urls =
    [
        new("file://C:\\test\\foo.md", typeof(Url)),
        new("file:///home/user/docs/text.md", typeof(Url)),
        new("https://www.gooooooooogle.com", typeof(Url)),
        new("http://127.0.0.1", typeof(Url)),
        new("ftp://ftp.someserver.invalidtld/?query=foo", typeof(Url)),
    ];
}
