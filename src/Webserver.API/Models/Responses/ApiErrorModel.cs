// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using System;

namespace Siemens.Simatic.S7.Webserver.API.Models.Responses
{
    /// <summary>
    /// An Api Error Model to represent the Api Response and include the Error => Throw the according Exception depending on the ErrorCode, provide further information about the request and the response 
    /// </summary>
    public class ApiErrorModel : BaseApiResponse
    {
        /// <summary>
        /// The ApiError given by the PLC - contains Code and Message
        /// </summary>
        public ApiError Error;

        /// <summary>
        /// Throws the Exception that suits the API Error Code or by default an ApiException if the ErrorCode Exception is not implemented
        /// </summary>
        /// <param name="apiRequestString"></param>
        /// <param name="responseString"></param>
        public void ThrowAccordingException(string apiRequestString, string responseString)
        {
            if (Error == null)
            {
                throw new NullReferenceException($"Api Error with errorcode has been expected but error in model was null - response from server:{responseString}");
            }
            switch (Error.Code)
            {
                case ApiErrorCode.InternalError:
                    throw new ApiInternalErrorException(new ApiException(this, apiRequestString));
                case ApiErrorCode.PermissionDenied:
                    throw new UnauthorizedAccessException("Token is invalid or missing or user has insufficient rights", new ApiException(this, apiRequestString));
                case ApiErrorCode.SystemIsBusy:
                    throw new ApiSystemIsBusyException(new ApiException(this, apiRequestString));
                case ApiErrorCode.NoResources:
                    throw new ApiNoResourcesException(new ApiException(this, apiRequestString));
                case ApiErrorCode.SystemIsReadOnly:
                    throw new ApiSystemIsReadOnlyException(new ApiException(this, apiRequestString));
                case ApiErrorCode.LoginFailed:
                    throw new UnauthorizedAccessException("invalid login parameters provided!", new ApiException(this, apiRequestString));
                case ApiErrorCode.AlreadyAuthenticated:
                    throw new ApiAlreadyAuthenticatedException(new ApiException(this, apiRequestString));
                case ApiErrorCode.AddresDoesNotExist:
                    throw new ApiAddresDoesNotExistException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidAddress:
                    throw new ApiInvalidAddressException(new ApiException(this, apiRequestString));
                case ApiErrorCode.VariableIsNotAStructure:
                    throw new ApiVariableIsNotAStructureException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidArrayIndex:
                    throw new ApiInvalidArrayIndexException(new ApiException(this, apiRequestString));
                case ApiErrorCode.UnsupportedAddress:
                    throw new ApiUnsupportedAddressException(new ApiException(this, apiRequestString));
                case ApiErrorCode.EntityDoesNotExist:
                    throw new ApiEntityDoesNotExistException(new ApiException(this, apiRequestString));
                case ApiErrorCode.EntityInUse:
                    throw new ApiEntityInUseException(new ApiException(this, apiRequestString));
                case ApiErrorCode.EntityAlreadyExists:
                    throw new ApiEntityAlreadyExistsException(new ApiException(this, apiRequestString));
                case ApiErrorCode.NotFound:
                    throw new ApiTicketNotFoundException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ApplicationNameAlreadyExists:
                    throw new ApiApplicationAlreadyExistsException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ApplicationDoesNotExist:
                    throw new ApiApplicationDoesNotExistException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ApplicationLimitReached:
                    throw new ApiApplicationLimitReachedException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidApplicationName:
                    throw new ApiInvalidApplicationNameException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ResourceContentIsNotReady:
                    throw new ApiResourceContentIsNotReadyException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ResourceVisibilityIsNotPublic:
                    throw new ApiResourceVisibilityIsNotPublicException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ResourceDoesNotExist:
                    throw new ApiResourceDoesNotExistException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ResourceAlreadyExists:
                    throw new ApiResourceAlreadyExistsException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidResourceName:
                    throw new ApiInvalidResourceNameException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ResourceLimitReached:
                    throw new ApiResourceLimitReachedException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidModificationTime:
                    throw new ApiInvalidModificationTimeException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidMediaType:
                    throw new ApiInvalidMediaTypeException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidETag:
                    throw new ApiInvalidETagException(new ApiException(this, apiRequestString));
                case ApiErrorCode.ResourceContentHasBeenCorrupted:
                    throw new ApiResourceContentHasBeenCorruptedException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidAlarmId:
                    throw new ApiInvalidAlarmIdException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidAlarmsBrowseParameters:
                    throw new ApiInvalidAlarmsBrowseParametersException(new ApiException(this, apiRequestString));
                case ApiErrorCode.PLCNotInStop:
                    throw new ApiPLCNotInStopException(new ApiException(this, apiRequestString));
                case ApiErrorCode.MethodNotFound:
                    throw new ApiMethodNotFoundException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidParams:
                    throw new ApiInvalidParametersException(new ApiException(this, apiRequestString));
                case ApiErrorCode.PasswordExpired:
                    throw new ApiPasswordExpiredException(new ApiException(this, apiRequestString));
                case ApiErrorCode.PasswordChangeNotAccepted:
                    throw new ApiPasswordChangeNotAcceptedException(new ApiException(this, apiRequestString));
                case ApiErrorCode.NewPasswordDoesNotFollowPolicy:
                    throw new ApiNewPasswordDoesNotFollowPolicyException(new ApiException(this, apiRequestString));
                case ApiErrorCode.NewPasswordMatchesOldPassword:
                    throw new ApiNewPasswordMatchesOldPasswordException(new ApiException(this, apiRequestString));
                case ApiErrorCode.PartnerNotAccessible:
                    throw new ApiPartnerNotAccessibleException(new ApiException(this, apiRequestString));
                case ApiErrorCode.NoServiceDataResources:
                    throw new ApiNoServiceDataResourcesException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidHwId:
                    throw new ApiInvalidHwIdException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidTimestamp:
                    throw new ApiInvalidTimestampException(new ApiException(this, apiRequestString));
                case ApiErrorCode.TimestampOutOfRange:
                    throw new ApiTimestampOutOfRangeException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidTimeRule:
                    throw new ApiInvalidTimeRuleException(new ApiException(this, apiRequestString));
                case ApiErrorCode.InvalidUTCOffset:
                    throw new ApiInvalidUTCOffsetException(new ApiException(this, apiRequestString));
                default:
                    throw new ApiException(this, apiRequestString);

            }
        }
    }
}
