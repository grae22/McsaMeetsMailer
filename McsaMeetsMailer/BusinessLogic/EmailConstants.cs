namespace McsaMeetsMailer.BusinessLogic
{
    public static class EmailConstants
    {
        public const string EmailSubject = "MCSA-KZN Meet Sheet";
        public const string EmailSubjectAbridged = "MCSA-KZN Meet Sheet - Upcoming meets!";

        public const string DefaultBody =
          "Hi\n\n" +
          "Please find the latest MCSA-KZN Meet Sheet below.\n\n" +
          "Queries specific to a particular Meet should be directed to the Meet Leader.\n\n" +
          "Please do not reply to this email as the address is not monitored.\n\n" +
          "Yours in adventure,\n" +
          "The Mountain Club of South Africa, KwaZulu-Natal Section.";

        public const string DefaultBodyAbridged =
          "Hi\n\n" +
          "Below you will find an abridged MCSA-KZN Meet Sheet, it lists meets in the upcoming weeks.\n\n" +
          "Queries specific to a particular Meet should be directed to the Meet Leader.\n\n" +
          "Please do not reply to this email as the address is not monitored.\n\n" +
          "Yours in adventure,\n" +
          "The Mountain Club of South Africa, KwaZulu-Natal Section.";
    }
}
