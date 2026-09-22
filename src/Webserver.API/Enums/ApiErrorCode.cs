// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT

namespace Siemens.Simatic.S7.Webserver.API.Enums
{
    /// <summary>
    /// Api Error Code given in the "Error" Json Object (by plc)
    /// </summary>
    public enum ApiErrorCode
    {
        /// <summary>
        /// Should never be the case!
        /// </summary>
        None = 0,
        /// <summary>
        /// Upon trying to execute the requested operation an internal error occured.
        /// Beim Versuch, die angeforderte Operation durchzuführen, ist ein interner Fehler aufgetreten.
        /// </summary>
        InternalError = 1,
        /// <summary>
        /// Permission denied
        /// </summary>
        PermissionDenied = 2,
        /// <summary>
        /// The requested operation cannot be executet since the system is currently processing another request.
        /// Send the request again once the currently processed request is completed
        /// It is also possible that this Error Message is returned when trying to close a ticket in busy state:
        /// The requested Ticket-ID is currently in State "busy" and cannot be deleted. Wait for the ticket to leave the "busy" state before calling the method again
        /// 
        /// Die gewünschte Operation kann nicht durgeführt werden, da das System im Moment eine
        /// andere Anfrage ausführt. Starten Sie die Anfrage erneut, sobald die laufende Operation
        /// abgeschlossen ist.
        /// Die angeforderte Ticket-ID befindet sich derzeit im Status "busy" und kann nicht gelöscht werden. 
        /// Warten Sie solange, bis das Ticket den Status "busy" verlassen hat, bevor Sie die Methode zum Löschen erneut aufrufen.
        /// </summary>
        SystemIsBusy = 3,
        /// <summary>
        /// The system does not have the necessary resources to execute the Web API request. 
        /// Execute the request again as soon as sufficient resources are available again.
        /// Some examples: you have
        ///  -reached the limit for logins (depending on plc) - wait 150 seconds and call the method (login) again.
        ///  -reached the limit for tickets for one user session or still a ticket for e.g. a download that is not closed yet. Close all open tickets in order to free resources and call this method again.
        ///  -system does generally not have the resources currently => wait for other requests to be completed
        /// Das System verfügt nicht über die nötigen Ressourcen, um den Web API-Request auszuführen 
        /// Führen Sie den Request erneut aus, sobald wieder genügend Ressourcen zur Verfügung stehen
        /// Sie haben alle Tickets in dieser Benutzersitzung ausgeschöpft. Schließen Sie vorhandene Tickets, um Ressourcen freizugeben. Wiederholen Sie danach den Aufruf der Methode.
        /// </summary>
        NoResources = 4,
        /// <summary>
        /// The system is currently in a write-protected state. Changes on Web Applications are currently not allowed. 
        /// Das System befindet sich derzeit in einem schreibgeschützten Zustand. Änderungen an Webapplikationen sind derzeit nicht zulässig.
        /// </summary>
        SystemIsReadOnly = 5,
        /// <summary>
        /// Not accepted in login: The authentication cannot be performed. This may happen because the requested mode is not supported by the PLC. <br/>
        /// Not accepted in password change: The password change cannot be performed. This is caused for example if an older PLC project is present where password changes are not supported.
        /// The execution of the requested method has not been accepted due to a method-specific restriction.
        /// </summary>
        NotAccepted = 6,
        /// <summary>
        /// The data of a PLC of an R/H system is not accessible.
        /// This may happen if the system is in state Syncup or RUN-redundant or if the service data of the partner PLC has been requested.
        /// </summary>
        PartnerNotAccessible = 7,
        /// <summary>
        /// The given credentials dont match any downloaded credentials to the plc.
        /// </summary>
        LoginFailed = 100,
        /// <summary>
        /// The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again
        /// Das aktuelle X-Auth-Token ist bereits authentifiziert. Verwenden Sie Api.Logout, bevor Sie sich erneut authentifizieren
        /// </summary>
        AlreadyAuthenticated = 101,
        /// <summary>
        /// The password of the user account has expired. The user needs to change the password to successfully authenticate again.
        /// </summary>
        PasswordExpired = 102,
        /// <summary>
        /// The provided new password does not match the required password policy.
        /// </summary>
        NewPasswordDoesNotFollowPolicy = 103,
        /// <summary>
        /// The provided new password is identical with the current password.
        /// </summary>
        NewPasswordMatchesOldPassword = 104,
        /// <summary>
        /// Authentication on the UMC server was not successful. This may happen due to various reasons, e.g. the UMC server could not be reached by the PLC.
        /// </summary>
        InfrastructureError = 105,
        /// <summary>
        /// The requested address does not exist or the webserver cannot access the requested address.
        /// Die angeforderte Adresse existiert nicht oder der Webserver kann nicht auf die angeforderte Adresse zugreifen.
        /// </summary>
        AddresDoesNotExist = 200,
        /// <summary>
        /// The structure of the given name to the symbolic address is not correct.
        /// Der Aufbau des Namens der symbolischen Adresse ist nicht korrekt.
        /// </summary>
        InvalidAddress = 201,
        /// <summary>
        /// Browsing the specific address is not possible since the given variable is not a structure.
        /// Das Durchsuchen der spezifischen Adresse ist nicht möglich, da die Variable kein strukturierter Datentyp ist.
        /// </summary>
        VariableIsNotAStructure = 202,
        /// <summary>
        /// The dimensions and edges of the array-indexes do not match the typeinformation of the plc. Did you provide a (correct) index?
        /// Die Dimensionen und Grenzen der Array-Indizes entsprechen nicht den Typinformationen der CPU.
        /// </summary>
        InvalidArrayIndex = 203,
        /// <summary>
        /// The datatype of the given address cannot be read or written.
        /// Der Datentyp der Adresse kann nicht gelesen werden.
        /// </summary>
        UnsupportedAddress = 204,
        /// <summary>
        /// The given address is read-only.
        /// Die angegebene Adresse kann nur gelesen werden.
        /// </summary>
        AddressIsReadOnly = 205,
        /// <summary>
        /// The provided path contains an illegal sequence
        /// </summary>
        PathIllegalSequence = 300,
        /// <summary>
        /// Access to the given entity is restricted
        /// </summary>
        EntityAccessRestricted = 301,
        /// <summary>
        /// The entity does not exist (e.g. Files Browse)
        /// </summary>
        EntityDoesNotExist = 302,
        /// <summary>
        /// The entity is already in use (e.g. Files.DeleteDirectory) 
        /// </summary>
        EntityInUse = 303,
        /// <summary>
        /// The entity already exists (e.g. Files CreateDirectory)
        /// </summary>
        EntityAlreadyExists = 304,
        /// <summary>
        /// The entity is not a directory
        /// </summary>
        EntityNotADirectory = 305,
        /// <summary>
        /// The entity is not a file
        /// </summary>
        EntityNotAFile = 306,
        /// <summary>
        /// The given path is too deep
        /// </summary>
        PathTooDeep = 307,
        /// <summary>
        /// Moving of the given entity is not allowed
        /// </summary>
        MoveNotLegal = 308,
        /// <summary>
        /// The given entity does not refer to an inactive datalog
        /// </summary>
        EntityNotInactiveDatalog = 309,
        /// <summary>
        /// Access to the given entity is denied
        /// </summary>
        EntityAccessDenied = 310,
        /// <summary>
        /// The given Ticket-ID is not found in the user(-token)s list of tickets
        /// </summary>
        NotFound = 400,
        /// <summary>
        /// An application with the given name already exists.
        /// Eine Applikation mit dem Namen existiert bereits. Vergeben Sie einen Namen, den es noch nicht gibt.
        /// </summary>
        ApplicationNameAlreadyExists = 500,
        /// <summary>
        /// There is no existing application with the given name. Create an application with the given name before calling this method.
        /// Eine Applikation mit diesem Namen existiert nicht. Vergeben Sie einen Namen, bevor Sie diese Methode aufrufen.
        /// </summary>
        ApplicationDoesNotExist = 501,
        /// <summary>
        /// The maximum amount of WebApps is reached. Delete unused or not needed Webapplications in order to free resources for new applications (or structure existing applications using /in resources names).
        /// Die maximale Anzahl an WebApplikationen ist erreicht. Löschen Sie nicht benötigte Applikationen, um Ressourcen für neue Applikationen freizugeben.
        /// </summary>
        ApplicationLimitReached = 502,
        /// <summary>
        /// The given application name is invalid. Set a name that is applying to the naming conventions for an application name from the manual.
        /// Der Name der Applikation ist ungültig. Vergeben Sie einen Applikationsnamen, der den Regeln für einen gültigen Applikationsnamen (Handbuch) entspricht.
        /// </summary>
        InvalidApplicationName = 503,
        /// <summary>
        /// The resource requested is not done processing earlier requests (for example it might still be uploaded,...). Try again later.
        /// </summary>
        ResourceContentIsNotReady = 504,
        /// <summary>
        /// The requested resource is not marked as "public". You should either change the resource to be "public" or request another resource that is already marked as "public".
        /// Die angeforderte Ressource ist nicht als "public" gekennzeichnet. Sie sollten die Ressource ändern in "public" oder eine andere Ressource auswählen, die bereits "public" ist.
        /// </summary>
        ResourceVisibilityIsNotPublic = 505,
        /// <summary>
        /// The requested resource does not exist inside the given application. Request a resource that exists in the application.
        /// Die angeforderte Ressource ist innerhalb der Applikation nicht vorhanden. Wählen Sie beim Aufrufen dieser Methode eine Ressource in der Applikation aus.
        /// </summary>
        ResourceDoesNotExist = 506,
        /// <summary>
        /// A resource with the given name already exists in this application. Either choose a new resource name or delete/rename the existing resource before calling this method again.
        /// Eine Ressource mit dem angegebenen Namen existiert bereits für diese Applikation. Wählen Sie einen neuen Ressourcennamen oder löschen/benennen Sie die Ressource um, bevor Sie diese Methode aufrufen.
        /// </summary>
        ResourceAlreadyExists = 507,
        /// <summary>
        /// The given resource name is invalid. Correct the resource name according to the manuals naming convention before calling this method again.
        /// Der neue Ressourcenname ist ungültig. Korrigieren Sie den Ressourcennamen (handbuch), bevor Sie diese Methode aufrufen.
        /// </summary>
        InvalidResourceName = 508,
        /// <summary>
        /// The maximum amount of resources has been reached for this application. Delete some resources before calling this method again.
        /// Die maximale Anzahl an Ressourcen ist für diese Applikation erschöpft. Löschen Sie einige Ressourcen, bevor Sie diese Methode aufrufen.
        /// </summary>
        ResourceLimitReached = 509,
        /// <summary>
        /// The version string provided does not meet the criteria of a valid version string.
        /// </summary>
        InvalidVersionString = 510,
        /// <summary>
        /// The proposed modification time is inalid. Adjust the proposed modification before calling this method again.
        /// Die vorgesehene Änderungszeit liegt außerhalb des zulässigen Zeitfensters für die Änderungszeit. Verringern Sie die Änderungszeit entsprechend, bevor Sie diese Methode aufrufen.
        /// </summary>
        InvalidModificationTime = 511,
        /// <summary>
        /// The proposed media type is inalid. Adjust the proposed media type before calling this method again.
        /// Der Medientyp ist ungültig. Korrigieren Sie den Medientyp (Seite 181), bevor Sie diese Methode aufrufen.
        /// </summary>
        InvalidMediaType = 512,
        /// <summary>
        /// The proposed ETag-value is inalid. Adjust the proposed ETag-value before calling this method again.
        /// Der ETag-Wert ist ungültig. Korrigieren Sie den Etag-Wert (Seite 202), bevor Sie diese Methode aufrufen.
        /// </summary>
        InvalidETag = 513,
        /// <summary>
        /// The content of the resource requested has been corrupted. Reupload the resource before calling this method again.
        /// </summary>
        ResourceContentHasBeenCorrupted = 514,
        /// <summary>
        /// Only one simultaneous ticket resource for service data across all users is possible at a time
        /// </summary>
        NoServiceDataResources = 600,
        /// <summary>
        /// The provided alarm ID is invalid.
        /// </summary>
        InvalidAlarmId = 800,
        /// <summary>
        /// The request is invalid. The user provided invalid parameters, e. g. alarm ID and count are present at the same time.
        /// </summary>
        InvalidAlarmParameters = 801,
        /// <summary>
        /// The provided timestamp does not match the required timestamp format
        /// </summary>
        InvalidTimestamp = 900,
        /// <summary>
        /// The timestamp is not within the allowed range
        /// </summary>
        TimestampOutOfRange = 901,
        /// <summary>
        /// The provided rule is invalid. Check the DaylightSavingsRule object. If the Rule object is present, both DST and STD is required.
        /// </summary>
        InvalidTimeRule = 902,
        /// <summary>
        /// The provided UTC offset is invalid. Check the main utc offset, and the DaylightSavingsRule object's DST offset.
        /// </summary>
        InvalidUtcOffset = 903,
        /// <summary>
        /// A backup creation is currently in progress
        /// </summary>
        BackupInProgress = 1000,
        /// <summary>
        /// A backup restoration is currently in progress
        /// </summary>
        RestoreInProgress = 1001,
        /// <summary>
        /// The memory card is write-protected
        /// </summary>
        MemoryCardWriteProtected = 1002,
        /// <summary>
        /// A backup restoration is not possible
        /// </summary>
        RestoreNotPossible = 1003,
        /// <summary>
        /// The PLC is not in operating mode stop. The method cannot be executed while the plc is not in stop mode.
        /// </summary>
        PLCNotInStop = 1004,
        /// <summary>
        /// Legitimation to continue the backup restoration failed
        /// </summary>
        LegitimationFailed = 1005,
        /// <summary>
        /// The requested hardware identifier is invalid
        /// </summary>
        InvalidHwId = 1100,
        /// <summary>
        /// The requested data record index is invalid
        /// </summary>
        IMDataInvalidIndex = 1101,
        /// <summary>
        /// The data for the requested hardware identifier is not readable
        /// </summary>
        IMdataNotReadable = 1102,
        /// <summary>
        /// Reading of I and M data is not supported for the requested hardware identifier
        /// </summary>
        IMdataNotSupported = 1103,
        /// <summary>
        /// The given default page is invalid cannot be set
        /// </summary>
        InvalidDefaultPage = 1300,
        /// <summary>
        /// The provided pattern is not in the allowed list. Choose a pattern that is allowed to be configured.
        /// </summary>
        InvalidPattern = 1301,
        /// <summary>
        /// At least one of the provided HTTP headers is not part in the allowed list. Choose a HTTP header that is allowed to be set.
        /// </summary>
        HTTPHeaderNotAllowed = 1302,
        /// <summary>
        /// At least one of the provided HTTP headers represents an invalid HTTP header. Verify the provided HTTP header string and correct it as required.
        /// </summary>
        HTTPHeaderInvalid = 1303,
        /// <summary>
        /// Too many HTTP response headers are configured. <br/>
        /// Reduce the number to a supported number of headers. The number is currently capped at 1.
        /// </summary>
        TooManyHTTPHeaders = 1304,
        /// <summary>
        /// The overall size of all HTTP headers requested to be configured is too large. <br/>
        /// The user shall either reduce the number of headers or the length of an individual HTTP header.
        /// </summary>
        RequestTooLarge = 1305,
        /// <summary>
        /// The accessed variable is not a variable of a technology object and cannot be read.
        /// </summary>
        NotATechnologyObject = 1400,
        /// <summary>
        /// The request cannot be performed while motion functionality is active
        /// </summary>
        MotionFunctionalityActive = 1401,
        /// <summary>
        /// The method request was invalid
        /// </summary>
        InvalidRequest = -32600,
        /// <summary>
        /// The method has not been found by the plc - check the spelling and fw-version (and according methods) of plc
        /// </summary>
        MethodNotFound = -32601,
        /// <summary>
        /// Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)
        /// </summary>
        InvalidParams = -32602,
        /// <summary>
        /// The given request could not be parsed successfully
        /// </summary>
        ParseError = -32700,
    }
}