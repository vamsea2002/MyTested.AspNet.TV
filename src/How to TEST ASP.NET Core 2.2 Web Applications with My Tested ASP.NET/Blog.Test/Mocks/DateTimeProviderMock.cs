namespace Blog.Test.Mocks
{
    using Blog.Services;
    using Moq;

    public class DateTimeProviderMock
    {
        public static IDateTimeProvider Instance
        {
            get
            {
                var mock = new Mock<IDateTimeProvider>();

                mock
                    .Setup(dtp => dtp.Now())
                    .Returns(new System.DateTime(2019, 9, 14));

                return mock.Object;
            }
        }
    }
}
