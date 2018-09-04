namespace NeverEmptyPantry.Common.Models.Account
{
    public class ProfileResult : Result
    {
        public ProfileDto Profile { get; set; }

        public static ProfileResult ProfileSuccess(ProfileDto profile) => new ProfileResult
        {
            Succeeded = true,
            Profile = profile
        };

        public static ProfileResult ProfileFailed(params Error[] errors) => new ProfileResult
        {
            Profile = null,
            Succeeded = false,
            Errors = errors
        };
    }
}