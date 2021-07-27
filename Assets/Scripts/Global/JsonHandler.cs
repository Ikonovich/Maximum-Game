using System;
using IO = System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


namespace MaxGame { 
	

	public static class JsonHandler {

		// Stores a dictionary containing the IDs and packed scenes of all objects implementing
		// IBuildable.


		static Dictionary<int, string> BuildIDs;

		// This stores a list of buildings that a playable unit can construct through the mouse interface.


		static JsonHandler() {

			string input = IO.File.ReadAllText(@"C:\Users\ikono\Documents\Maximum Game\Assets\JSON\BuildIDs\AllBuildIDs");

			BuildIDs = JsonConvert.DeserializeObject<Dictionary<int, string>>(input);


		}


		public static List<int> GetBuildableList(TextAsset buildIDs) {

		 	string input = buildIDs.ToString();

			List<int> output = JsonConvert.DeserializeObject<List<int>>(input);

			Console.WriteLine("Returning available buildable IDs from Json Handler");

			return output;

		}

		public static void ExportToJson(Dictionary<string, string> dict, string path) {


			string output = JsonConvert.SerializeObject(dict);

			IO.File.WriteAllText(path, output);
			
		}

		public static Dictionary<int, string> GetBuildTable() {
			
			
			Console.WriteLine("Returing all buildable IDs from Json Handler");


			return BuildIDs;

		}
	}
}
