namespace LSpacesProject
{
	using SimpleSQL;

	public class LSpaces
	{
		[PrimaryKey]
		public int LSpaceID { get; set; }

		public string LSName { get; set; }

		public int LSSampleSize { get; set; }

		public int LSDimSize { get; set; }

		public int LSImageWidth { get; set; }

		public int LSImageHeight { get; set; }
	}
}