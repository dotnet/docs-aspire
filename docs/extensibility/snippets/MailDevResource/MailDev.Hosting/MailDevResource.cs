// For ease of discovery, resource types should be placed in
// the Aspire.Hosting.ApplicationModel namespace. If there is
// likelihood of a conflict on the resource name consider using
// an alternative namespace.
namespace Aspire.Hosting.ApplicationModel;

public sealed class MailDevResource(
    string name,
    ParameterResource? username,
    ParameterResource password)
        : ContainerResource(name), IResourceWithConnectionString
{
    // Constants used to refer to well known-endpoint names, this is specific
    // for each resource type. MailDev exposes an SMTP and HTTP endpoints.
    internal const string SmtpEndpointName = "smtp";
    internal const string HttpEndpointName = "http";

    private const string DefaultUsername = "mail-dev";

    // An EndpointReference is a core .NET Aspire type used for keeping
    // track of endpoint details in expressions. Simple literal values cannot
    // be used because endpoints are not known until containers are launched.
    private EndpointReference? _smtpReference;

    /// <summary>
    /// Gets the parameter that contains the MailDev SMTP server username.
    /// </summary>
    public ParameterResource? UsernameParameter { get; } = username;

    internal ReferenceExpression UserNameReference =>
        UsernameParameter is not null ?
        ReferenceExpression.Create($"{UsernameParameter}") :
        ReferenceExpression.Create($"{DefaultUsername}");

    /// <summary>
    /// Gets the parameter that contains the MailDev SMTP server password.
    /// </summary>
    public ParameterResource PasswordParameter { get; } = password;

    public EndpointReference SmtpEndpoint =>
        _smtpReference ??= new(this, SmtpEndpointName);

    // Required property on IResourceWithConnectionString. Represents a connection
    // string that applications can use to access the MailDev server. In this case
    // the connection string is composed of the SmtpEndpoint endpoint reference.
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"Endpoint=smtp://{SmtpEndpoint.Property(EndpointProperty.Host)}:{SmtpEndpoint.Property(EndpointProperty.Port)};Username={UserNameReference};Password={PasswordParameter}"
        );
}
