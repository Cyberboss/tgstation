using System.ServiceModel;

namespace TGServiceInterface
{
	[ServiceContract]
	public interface ITGCompiler
	{
		//Sets up the symlinks for hotswapping game code
		//this will reset everything and terminate dream daemon if it is running
		//this will not reset the static directories
		//requires the repository to be set up and locks it for the duration of the operation
		//does not compiler the game
		//returns null on success, error on failure
		[OperationContract]
		string Initialize();

		//Does all the necessary actions to take the revision currently in the repository
		//and compile it to be run on the next server reboot
		//requires byond to be set up
		//runs asyncronously
		//returns true if the operation began, false if compilation is already in progress
		[OperationContract]
		bool Compile();

		//Returns true if there are compilation/directory operations in progress
		//false otherwise
		[OperationContract]
		bool Compiling();

		//Returns true if the last compilation was successful
		//false otherwise
		//Value meaningless if Compiling returns true
		[OperationContract]
		bool Compiled();
	}
}
