namespace LSpacesProject
{
	using SimpleSQL;

	public class LSPoints
	{
		[PrimaryKey]
		public int LSpaceID { get; set; }
		[PrimaryKey]
		public int LSPointID { get; set; }

		public string LSPointName { get; set; }

		public byte[] LSPointPos { get; set; }

		public byte[] LSPointStd { get; set; }

		public byte[] LSPointImage { get; set; }
	}
}