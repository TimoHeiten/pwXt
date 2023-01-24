namespace pwXt_Service.ValueObjects;
/*
 * to not use the wrong Primitive value
 */

public record PasswordId(string Value)
{
    public static PasswordId Empty => new("");
}

public record PasswordValue(string Value)
{
    public static PasswordValue Empty => new("");
}