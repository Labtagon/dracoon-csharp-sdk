﻿using System.Collections.Generic;
using Dracoon.Sdk.Model;
using Dracoon.Sdk.SdkInternal.ApiModel;
using Dracoon.Sdk.SdkInternal.Util;

namespace Dracoon.Sdk.SdkInternal.Mapper {
    internal static class SettingsMapper {
        internal static ServerGeneralSettings FromApiGeneralSettings(ApiGeneralSettings apiGeneralConfig) {
            if (apiGeneralConfig == null) {
                return null;
            }

            ServerGeneralSettings general = new ServerGeneralSettings {
                CryptoEnabled = apiGeneralConfig.CryptoEnabled,
                EmailNotificationButtonEnabled = apiGeneralConfig.EmailNotificationButtonEnabled,
                EulaEnabled = apiGeneralConfig.EulaEnabled,
                MediaServerEnabled = apiGeneralConfig.MediaServerEnabled,
                SharePasswordSmsEnabled = apiGeneralConfig.SharePasswordSmsEnabled,
                UseS3Storage = apiGeneralConfig.UseS3Storage,
                WeakPasswordEnabled = apiGeneralConfig.WeakPasswordEnabled
            };
            return general;
        }

        internal static ServerInfrastructureSettings FromApiInfrastructureSettings(ApiInfrastructureSettings apiInfrastructureConfig) {
            if (apiInfrastructureConfig == null) {
                return null;
            }

            ServerInfrastructureSettings infrastructure = new ServerInfrastructureSettings {
                MediaServerConfigEnabled = apiInfrastructureConfig.MediaServerConfigEnabled,
                S3DefaultRegion = apiInfrastructureConfig.S3DefaultRegion,
                SmsConfigEnabled = apiInfrastructureConfig.SmsConfigEnabled,
                S3EnforceDirectUpload = apiInfrastructureConfig.S3EnforceDirectUpload
            };
            return infrastructure;
        }

        internal static ServerDefaultSettings FromApiDefaultsSettings(ApiDefaultsSettings apiDefaultsConfig) {
            if (apiDefaultsConfig == null) {
                return null;
            }

            ServerDefaultSettings defaults = new ServerDefaultSettings {
                LanguageDefault = apiDefaultsConfig.LanguageDefault,
                DownloadShareDefaultExpirationPeriodInDays = apiDefaultsConfig.DownloadShareDefaultExpirationPeriodInDays,
                FileUploadDefaultExpirationPeriodInDays = apiDefaultsConfig.FileUploadDefaultExpirationPeriodInDays,
                UploadShareDefaultExpirationPeriodInDays = apiDefaultsConfig.UploadShareDefaultExpirationPeriodInDays
            };
            return defaults;
        }

        internal static PasswordPolicies FromApiPasswordPolicies(ApiPasswordSettings apiPolicies) {
            if (apiPolicies == null) {
                return null;
            }

            PasswordPolicies policies = new PasswordPolicies {
                EncryptionPolicies = FromApiPasswordEncryptionPolicies(apiPolicies.EncryptionPasswordSettings),
                LoginPolicies = FromApiPasswordLoginPolicies(apiPolicies.LoginPasswordSettings),
                SharePolicies = FromApiPasswordSharePolicies(apiPolicies.SharePasswordSettings)
            };
            return policies;
        }

        internal static PasswordSharePolicies FromApiPasswordSharePolicies(ApiSharePasswordSettings apiPolicies) {
            if (apiPolicies == null) {
                return null;
            }

            PasswordSharePolicies policies = new PasswordSharePolicies {
                CharacterPolicies = FromApiPasswordCharacterPolicies(apiPolicies.CharacterRules),
                MinimumPasswordLength = apiPolicies.MinimumPasswordLength,
                RejectDictionaryWords = apiPolicies.RejectDictionaryWords,
                RejectKeyboardPatterns = apiPolicies.RejectKeyboardPatterns,
                RejectOwnUserInfo = apiPolicies.RejectUserInfo,
                UpdatedAt = apiPolicies.UpdatedAt,
                UpdatedBy = UserMapper.FromApiUserInfo(apiPolicies.UpdatedBy)
            };
            return policies;
        }

        internal static PasswordLoginPolicies FromApiPasswordLoginPolicies(ApiLoginPasswordSettings apiPolicies) {
            if (apiPolicies == null) {
                return null;
            }

            PasswordLoginPolicies policies = new PasswordLoginPolicies {
                CharacterPolicies = FromApiPasswordCharacterPolicies(apiPolicies.CharacterRules),
                MinimumPasswordLength = apiPolicies.MinimumPasswordLength,
                RejectDictionaryWords = apiPolicies.RejectDictionaryWords,
                RejectKeyboardPatterns = apiPolicies.RejectKeyboardPatterns,
                RejectOwnUserInfo = apiPolicies.RejectUserInfo,
                NumberOfArchivedPasswords = apiPolicies.NumberOfArchivedPasswords,
                UpdatedAt = apiPolicies.UpdatedAt,
                UpdatedBy = UserMapper.FromApiUserInfo(apiPolicies.UpdatedBy),
                LoginFailurePolicies = FromApiPasswordLoginFailurePolicies(apiPolicies.UserLockoutRules),
                PasswordExpirationPolicies = FromApiPasswordExpirationPolicies(apiPolicies.PasswordExpirationRules)
            };
            return policies;
        }

        internal static PasswordEncryptionPolicies FromApiPasswordEncryptionPolicies(ApiEncryptionPasswordSettings apiPolicies) {
            if (apiPolicies == null) {
                return null;
            }

            PasswordEncryptionPolicies policies = new PasswordEncryptionPolicies {
                CharacterPolicies = FromApiPasswordCharacterPolicies(apiPolicies.CharacterRules),
                MinimumPasswordLength = apiPolicies.MinimumPasswordLength,
                RejectKeyboardPatterns = apiPolicies.RejectKeyboardPatterns,
                RejectOwnUserInfo = apiPolicies.RejectUserInfo,
                UpdatedAt = apiPolicies.UpdatedAt,
                UpdatedBy = UserMapper.FromApiUserInfo(apiPolicies.UpdatedBy)
            };
            return policies;
        }

        internal static PasswordExpiration FromApiPasswordExpirationPolicies(ApiPasswordExpiration apiPolicies) {
            if (apiPolicies == null) {
                return null;
            }

            PasswordExpiration policies = new PasswordExpiration {
                IsEnabled = apiPolicies.ExpirationIsEnabled,
                ExpiresAfterDays = apiPolicies.MaximumPasswordAgeInDays
            };
            return null;
        }

        internal static PasswordLoginFailurePolicies FromApiPasswordLoginFailurePolicies(ApiUserLockout apiPolicies) {
            if (apiPolicies == null) {
                return null;
            }

            PasswordLoginFailurePolicies policies = new PasswordLoginFailurePolicies {
                IsEnabled = apiPolicies.Enabled,
                LoginRetryPeriodMinutes = apiPolicies.MinutesToNextLoginAttempt,
                MaximumNumberOfLoginFailures = apiPolicies.MaximumLoginFailureAttempts
            };
            return policies;
        }

        internal static PasswordCharacterPolicies FromApiPasswordCharacterPolicies(ApiCharacterRules apiPolicies) {
            if (apiPolicies == null) {
                return null;
            }

            PasswordCharacterPolicies policies = new PasswordCharacterPolicies {
                NumberOfMustContainCharacteristics = apiPolicies.NumberOfCharacteristicsToEnforce
            };

            policies.PredefinedCharacterSets = new List<PasswordCharacterSet>();
            foreach (string current in apiPolicies.MustContainCharacters) {
                if (current != "alpha") {
                    PasswordCharacterSetType type = EnumConverter.ConvertValueToCharacterSetTypeEnum(current);
                    policies.PredefinedCharacterSets.Add(new PasswordCharacterSet {
                        Type = type,
                        Set = GeneratePasswordPoliciesSet(type)
                    });
                }
            }

            // Convert "alpha" value to uppercase & lowercase if one of them are not included yet
            if (apiPolicies.MustContainCharacters.Contains("alpha")) {
                if (!apiPolicies.MustContainCharacters.Contains("lowercase")) {
                    PasswordCharacterSetType type = PasswordCharacterSetType.Lowercase;
                    policies.PredefinedCharacterSets.Add(new PasswordCharacterSet {
                        Type = type,
                        Set = GeneratePasswordPoliciesSet(type)
                    });
                }

                if (!apiPolicies.MustContainCharacters.Contains("uppercase")) {
                    PasswordCharacterSetType type = PasswordCharacterSetType.Uppercase;
                    policies.PredefinedCharacterSets.Add(new PasswordCharacterSet {
                        Type = type,
                        Set = GeneratePasswordPoliciesSet(type)
                    });
                }
            }

            return policies;
        }

        internal static char[] GeneratePasswordPoliciesSet(PasswordCharacterSetType type) {
            switch (type) {
                case PasswordCharacterSetType.Lowercase:
                    return ApiConfig.LOWERCASE_SET;
                case PasswordCharacterSetType.Uppercase:
                    return ApiConfig.UPPERCASE_SET;
                case PasswordCharacterSetType.Numeric:
                    return ApiConfig.NUMERIC_SET;
                case PasswordCharacterSetType.Special:
                    return ApiConfig.SPECIAL_SET;
                default:
                    return new char[0];
            }
        }
    }
}