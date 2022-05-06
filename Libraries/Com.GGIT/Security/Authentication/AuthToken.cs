using Com.GGIT.Common;
using Com.GGIT.Security.JsonWebToken;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Com.GGIT.Security.Authentication
{
    public class AuthToken
    {
        private Logger _log;
        protected Logger Log => _log ?? (_log = LogManager.GetCurrentClassLogger());

        // Constant variables
        private const string AccessType = "access";
        private const string IdType = "id";
        private const string Sub = "sub"; // refers to user globalguid
        private const string Ace_Elev_Exp = "ace_elev_exp";
        private const string Ace_Email = "email";
        private const string Ace_Ref = "ace_ref";
        private const string Ace_TokenUse = "token_use";
        private const string Ace_Username = "username";
        private const string Merchant = "m";
        private const string MerchantName = "merchant";
        private const string Consumer = "c";
        private const string ConsumerName = "consumer";
        private const string Ace_Depo = "ace_depo";
        private const string Ace_Client_Id = "client_id";
        private const string Ace_Role = "ace_role";
        private const string Ace_Scope = "scope";

        private readonly string _AccessToken;
        private readonly string _IdentityToken;
        private readonly TokenDto AccessTokenObject;
        private readonly TokenDto IdentityTokenObject;
        private JwtSecurityTokenHandler SecurityTokenHandler;
        private JwtSecurityToken SecurityToken;

        public string ErrorMessage { get; set; }
        public bool AuthTokenValid { get; set; }
        public string JwtTokenGuid { get; set; }
        public string Email { get; set; }
        public string ReferralGuid { get; set; }
        public string UserGuid { get; set; }
        public string Username { get; set; }
        public bool IsDeposit { get; set; }
        public bool IsUSCitizen { get; set; }
        public string UserRole { get; set; }
        public string RoleName { get; set; }
        public string Client_Id { get; set; }
        public string Scope { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Function to validate access token
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="CheckExpired"></param>
        /// <param name="CheckTms"></param>
        /// <param name="Timestamp"></param>
        /// <param name="IsMilliseconds"></param>
        public AuthToken(string AccessToken, bool CheckExpired = true, bool CheckTms = false, long? Timestamp = null, bool IsMilliseconds = false)
        {
            _AccessToken = AccessToken;
            if (!ValidateToken(_AccessToken, CheckExpired)) return;

            // validate success and proceed extract information
            AccessTokenObject = ExtractTokenAttributes(_AccessToken, AccessType);

            // Load/manipulate information to AuthToken attributes
            if (!AccessTokenObject.IsValid) return;
            else
            {
                // validate timestamp
                if (CheckTms
                    && TokenExpired(timestamp: Timestamp.Value, tokenExp: AccessTokenObject.Expiry, IsMilliseconds: true))
                {
                    Log.Error("Token is expired. Transaction timestamp: " + Timestamp.Value + ".Jwt expiry timestamp: " + AccessTokenObject.Expiry);
                    return;
                }
                Log.Info("Token validation success.");
                AuthTokenValid = true;
            }
        }

        /// <summary>
        /// Function to validate id token and access token, mostly use for action(withdrawal, loan and etc) security validation and registration purpose
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="IdentityToken"></param>
        /// <param name="CheckExpired"></param>
        /// <param name="ElevatedExpiry"></param>
        /// <param name="CheckTms"></param>
        /// <param name="Timestamp"></param>
        /// <param name="IsMilliseconds"></param>
        /// <param name="IsRegistration"></param>
        public AuthToken(string AccessToken, string IdentityToken, bool CheckExpired = true, bool ElevatedExpiry = false, bool CheckTms = false, long? Timestamp = null, bool IsMilliseconds = false, bool IsRegistration = false)
        {
            /*Validate id_token*/
            _IdentityToken = IdentityToken;
            if (!ValidateToken(_IdentityToken, CheckExpired)) return;
            // validate success and proceed extract information
            IdentityTokenObject = ExtractTokenAttributes(_IdentityToken, IdType, ElevatedExpiry, IsRegistration);

            /*Validate access_token*/
            _AccessToken = AccessToken;
            if (!ValidateToken(_AccessToken, CheckExpired)) return;
            // validate success and proceed extract information
            AccessTokenObject = ExtractTokenAttributes(_AccessToken, AccessType);

            // Load/manipulate information to AuthToken attributes
            if (!AccessTokenObject.IsValid || !IdentityTokenObject.IsValid) return;
            else
            {
                // validate timestamp, if expired, return AuthTokenValid with FALSE. Either token expired, exit.
                if (CheckTms
                    && (TokenExpired(timestamp: Timestamp.Value, tokenExp: IdentityTokenObject.Expiry, IsMilliseconds: true)
                    || TokenExpired(timestamp: Timestamp.Value, tokenExp: AccessTokenObject.Expiry, IsMilliseconds: true)))
                {
                    Log.Error("Token is expired. Transaction timestamp: " + Common.Util.DatetimeUtil.FromUnixTimeMilliseconds(Timestamp.Value) +
                        ".Id jwt expiry timestamp: " + Common.Util.DatetimeUtil.FromUnixTimeSeconds(IdentityTokenObject.Expiry) +
                        ".Access jwt expiry timestamp: " + Common.Util.DatetimeUtil.FromUnixTimeSeconds(AccessTokenObject.Expiry));
                    return;
                }

                Log.Info("Token validation success.");
                AuthTokenValid = ManipulateInformation(IsRegistration);
            }
        }

        private bool ManipulateInformation(bool IsRegistration)
        {
            if (!IsRegistration) return true; // Only registration will check user role

            if (string.IsNullOrEmpty(UserRole))
            {
                ErrorMessage = "Missing user role";
                return false;
            }

            if (UserRole.EqualsIgnoreCase(Merchant))
            {
                IsUSCitizen = true; // update US citizen flag when user role is merchant.
                IsDeposit = false;
            }
            return true;
        }

        private bool ValidateToken(string token, bool expiry = true, bool audience = false, bool actor = false, bool issuer = false)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            if (JwtTokenHandler.CanReadToken(token))
            {
                IdentityModelEventSource.ShowPII = true;
                string KeyId = JwtTokenHandler.ReadJwtToken(token).Header.Kid;
                JwtKeys Jkey = JwtSettings.GetKey();
                try
                {
                    if (Jkey.IsNull()) throw new Exception("Public key not found exception.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return false;
                }
                JsonWebKey jwtKey = SearchValidKeyId(Jkey.Keys, KeyId);
                var parameters = new TokenValidationParameters
                {
                    ValidIssuer = Jkey.Issuer,             //issuer URL
                    IssuerSigningKey = jwtKey,             //public key
                    ValidateLifetime = expiry,
                    ValidateAudience = audience,
                    ValidateActor = actor,
                    ValidateIssuer = issuer
                };

                try
                {
                    /* Start to get some information before validate */
                    var Payload = JwtTokenHandler.ReadJwtToken(token).Payload;
                    // User globalguid
                    if (string.IsNullOrEmpty(UserGuid) && Payload.ContainsKey(Sub))
                    {
                        Payload.TryGetTypedValue(Sub, out string value);
                        UserGuid = value;
                    }
                    // User's referral globalguid
                    if (string.IsNullOrEmpty(ReferralGuid) && Payload.ContainsKey(Ace_Ref))
                    {
                        Payload.TryGetTypedValue(Ace_Ref, out string value);
                        ReferralGuid = value;
                    }
                    // User's username
                    if (string.IsNullOrEmpty(Username) && Payload.ContainsKey(Ace_Username))
                    {
                        Payload.TryGetTypedValue(Ace_Username, out string value);
                        Username = value;
                    }
                    /* End */
                    var ClaimsPrincipal = JwtTokenHandler.ValidateToken(token, parameters, out SecurityToken securityToken);
                    return true;
                }
                catch (SecurityTokenExpiredException ex)
                {
                    //Token is expired
                    ErrorMessage = "Token expired.";
                    Log.Error(string.Format("Token: {0}\n>> Error Message: {1}", token, ex.Message));
                }
                catch (SecurityTokenSignatureKeyNotFoundException ex)
                {
                    //Signature validation failed, token's kid not match with publickey's kid
                    ErrorMessage = "Invalid Signature.";
                    Log.Error(string.Format("Token: {0}\n>> Error Message: {1}", token, ex.Message));
                }
                catch (SecurityTokenInvalidAudienceException ex)
                {
                    //Client Id mismatch
                    ErrorMessage = "Invalid Client Id.";
                    Log.Error(string.Format("Token: {0}\n>> Error Message: {1}", token, ex.Message));
                }
                catch (SecurityTokenException ex)
                {
                    ErrorMessage = "Unhandled exception.";
                    Log.Error(string.Format("Token: {0}\n>> Error Message: {1}", token, ex.Message));
                }
            }
            return false;
        }

        private JsonWebKey SearchValidKeyId(IList<PublicKeys> keys, string KeyId)
        {
            PublicKeys _publicKey = null;
            foreach (var item in keys)
            {
                if (string.Equals(item.Kid, KeyId))
                {
                    _publicKey = item;
                    break;
                }
            }
            if (_publicKey == null) return null;

            return new JsonWebKey
            {
                Kid = _publicKey.Kid,
                Alg = _publicKey.Alg,
                E = _publicKey.E,
                Kty = _publicKey.Kty,
                N = _publicKey.N,
                Use = _publicKey.Use
            };
        }

        private TokenDto ExtractTokenAttributes(string token, string tokenType, bool ElevatedExpiry = false, bool IsRegistration = false)
        {
            SecurityTokenHandler = new JwtSecurityTokenHandler();
            TokenDto tokenDto = new TokenDto();

            if (SecurityTokenHandler.CanReadToken(token))
            {
                SecurityToken = SecurityTokenHandler.ReadJwtToken(token);
                var TokenPayload = SecurityToken.Payload;

                // Jwt identifier
                if (!string.IsNullOrEmpty(TokenPayload.Jti))
                {
                    tokenDto.JwtTokenGuid = TokenPayload.Jti;
                    JwtTokenGuid = TokenPayload.Jti;
                }
                // Jwt expiry timestamp
                tokenDto.Expiry = TokenPayload.Exp.HasValue ? Convert.ToInt64(TokenPayload.Exp) : 0;
                // Token type
                if (TokenPayload.ContainsKey(Ace_TokenUse))
                {
                    TokenPayload.TryGetTypedValue(Ace_TokenUse, out string value);
                    if (tokenType.EqualsIgnoreCase(value))
                    {
                        tokenDto.TokenType = value;
                        tokenDto.IsValid = true;
                    }
                    if (!tokenDto.IsValid)
                    {
                        ErrorMessage = "Invalid token type. Expecting \"" + tokenType + "\" but this token's type is \"" + value + "\".";
                        Log.Error("Token: " + token + "\n>> Invalid Token Type. We are expecting \"" + tokenType + "\" token type.");
                        return tokenDto;
                    }
                }

                if (IsRegistration)
                {
                    if (TokenPayload.ContainsKey(Sub))
                    {
                        TokenPayload.TryGetTypedValue(Sub, out string value);
                        UserGuid = value;
                    }

                    // WilliamHo 20201015 Prevent given jwt sub or ACE user globalguid given in empty or null, avoid mistakenly insert into database
                    if (string.IsNullOrWhiteSpace(UserGuid))
                    {
                        ErrorMessage = "Missing GlobalGuid - Given GlobalGuid either NULL or EMPTY";
                        Log.Error("Token: " + token + "\n>> Missing GlobalGuid - Given GlobalGuid either NULL or EMPTY");
                        tokenDto.IsValid = false;
                        return tokenDto;
                    }

                    // User Email
                    if (TokenPayload.ContainsKey(Ace_Email))
                    {
                        TokenPayload.TryGetTypedValue(Ace_Email, out string value);
                        tokenDto.UserEmail = value;
                        Email = value;
                    }
                    // User role, c = consumer, m = merchant
                    if (TokenPayload.ContainsKey(Ace_Role))
                    {
                        TokenPayload.TryGetTypedValue(Ace_Role, out string value);
                        tokenDto.Userrole = value;
                        UserRole = value;
                        switch (value)
                        {
                            case Consumer:
                                RoleName = ConsumerName;
                                break;
                            case Merchant:
                                RoleName = MerchantName;
                                break;
                            default:
                                Log.Error("Token: " + token + "\n>> Invalid user role in JWT.");
                                throw new Exception("Invalid user role in JWT.");
                        }
                    }
                    else
                    {
                        ErrorMessage = "Missing claims - ace_role";
                        Log.Error("Token: " + token + "\n>> Missing claims - ace_role");
                        tokenDto.IsValid = false;
                        return tokenDto;
                    }
                    // User able deposit or US citizen; if deposit = true, then US citizen = false, and vice versa
                    if (TokenPayload.ContainsKey(Ace_Depo))
                    {
                        TokenPayload.TryGetTypedValue(Ace_Depo, out string value);
                        bool tmp = Convert.ToBoolean(value);
                        tokenDto.AllowDeposit = tmp;
                        tokenDto.USCitizen = !tmp;
                        IsDeposit = tmp;
                        IsUSCitizen = !tmp;
                    }
                    else
                    {
                        ErrorMessage = "Missing claims - ace_depo";
                        Log.Error("Token: " + token + "\n>> Missing claims - ace_depo");
                        tokenDto.IsValid = false;
                        return tokenDto;
                    }
                }

                // User scope
                if (TokenPayload.ContainsKey(Ace_Scope))
                {
                    TokenPayload.TryGetTypedValue(Ace_Scope, out string value);
                    tokenDto.Scope = value;
                    Scope = value;
                }
                // Token client id
                if (TokenPayload.ContainsKey(Ace_Client_Id))
                {
                    TokenPayload.TryGetTypedValue(Ace_Client_Id, out string value);
                    tokenDto.ClientId = value;
                    Client_Id = value;
                }

                // Token elevated expiry, generated during QRcode/Passphrase
                if (ElevatedExpiry)
                {
                    if (TokenPayload.ContainsKey(Ace_Elev_Exp))
                    {
                        TokenPayload.TryGetValue(Ace_Elev_Exp, out object value);
                        var TokenExpiryTime = Convert.ToInt64(value);
                        var CurrentDatetime = Common.Util.DatetimeUtil.GetEpochSeconds();

                        if (TokenExpired(CurrentDatetime, TokenExpiryTime))
                        {
                            ErrorMessage = "Token elevate expired";
                            Log.Error("Token: " + token + "\n>> Token elevate expired"
                                + ". Current datetime: " + Common.Util.DatetimeUtil.FromUnixTimeSeconds(CurrentDatetime)
                                + ". Elevate datetime: " + Common.Util.DatetimeUtil.FromUnixTimeSeconds(TokenExpiryTime));
                            tokenDto.IsValid = false;
                        }
                    }
                    else // When "ace_elev_exp" not found, return invalid token error during withdrawal/wholesale process.
                    {
                        ErrorMessage = "Missing claims - ace_elev_exp";
                        Log.Error("Token: " + token + "\n>> Missing claims - ace_elev_exp");
                        tokenDto.IsValid = false;
                    }
                }
            }
            return tokenDto;
        }

        /// <summary>
        /// Validate header timestamp against token expiry timestamp
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="tokenExp"></param>
        /// <param name="IsMilliseconds"></param>
        /// <returns></returns>
        /// <remarks>
        /// "timestamp" referring to current timestamp or transaction sent timestamp.
        /// During consume RabbitMQ transaction, timestamp_in_ms contain milliseconds and require to truncate up to seconds.
        /// Check against timestamp(created/sent) and token expiry timestamp.
        /// </remarks>
        private bool TokenExpired(long timestamp, long tokenExp, bool IsMilliseconds = false)
        {
            if (IsMilliseconds) timestamp = Common.Util.DatetimeUtil.ConvertUnixMillisecondsToSeconds(timestamp);
            return Common.Util.DatetimeUtil.ExpiredUnixTimeSeconds(timestamp, tokenExp);
        }
    }

    public class TokenDto
    {
        public bool IsValid { get; set; }
        public long Expiry { get; set; }
        public string TokenType { get; set; }
        public string UserGuid { get; set; }
        public string ReferralGuid { get; set; }
        public string UserEmail { get; set; }
        public string Username { get; set; }
        public string JwtTokenGuid { get; set; }
        public bool USCitizen { get; set; }
        public bool AllowDeposit { get; set; }
        public string Userrole { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
    }
}
