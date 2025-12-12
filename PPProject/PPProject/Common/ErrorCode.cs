namespace PPProject.Common
{
    public class ErrorCode
    {
        /// <summary>
        /// 패러미터 검증 실패
        /// </summary>
        public const int PARAMETER_VALIDATION_FAILED = 10001;
        /// <summary>
        /// 세션 검증 실패
        /// </summary>
        public const int SESSION_VALIDATION_FAILED = 10002;
        /// <summary>
        /// 구글 로그인 검증 실패
        /// </summary>
        public const int GOOGLE_AUTH_VALIDATION_FAILED = 10003;
        /// <summary>
        /// 지원하지 않는 플랫폼
        /// </summary>
        public const int NOT_SUPPORTED_PLATFORM = 20001;
    }
}
