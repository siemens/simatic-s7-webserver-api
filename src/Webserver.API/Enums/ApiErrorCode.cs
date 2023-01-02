// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        ///  -reached the limit for logins (depending on plc) - wait a maximum of 120 seconds and call the method (login) again.
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
        /// The given credentials dont match any downloaded credentials to the plc.
        /// </summary>
        LoginFailed = 100,
        /// <summary>
        /// The given X-Auth-Token is already authenticated. Use Api.Logout before logging in again
        /// Das aktuelle X-Auth-Token ist bereits authentifiziert. Verwenden Sie Api.Logout, bevor Sie sich erneut authentifizieren
        /// </summary>
        AlreadyAuthenticated = 101,
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
        /// The method has not been found by the plc - check the spelling and fw-version (and according methods) of plc
        /// </summary>
        MethodNotFound = -32601,
        /// <summary>
        /// Invalid Parameters provided (null/empty string that is forbidden?, invalid ticket length?, wrong datetime string format (rfc3339)? ...)
        /// </summary>
        InvalidParams = -32602



    }
}
