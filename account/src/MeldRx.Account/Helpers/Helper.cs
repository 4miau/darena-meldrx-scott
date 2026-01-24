namespace MeldRx.Account.Helpers
{
    public class Helper
    {
        #region Gender Image Method
        public static string GenderImage(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return Assets.DummyProfile;
                case Gender.Female:
                    return Assets.FemaleDummy;
                default:
                    return Assets.FemaleDummy;
            }
        }
        #endregion
    }
}
