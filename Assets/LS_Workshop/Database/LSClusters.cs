namespace LSpacesProject
{
	using SimpleSQL;

	public class LSClusters
	{
		[PrimaryKey]
		public int LSpaceID { get; set; }
		[PrimaryKey]
		public string LSClusterName { get; set; }

		public byte[] LSClusterMap { get; set; }

		public string LSClusterLabels { get; set; }
	}
}