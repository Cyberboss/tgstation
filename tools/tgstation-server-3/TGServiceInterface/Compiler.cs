using System.ServiceModel;

namespace TGServiceInterface
{
	public enum TGCompilerStatus
	{
		Uninitialized,
		Initializing,
		Initialized,
		Compiling,
	}
	[ServiceContract]
	public interface ITGCompiler
	{
		//Sets up the symlinks for hotswapping game code
		//this will reset everything and terminate dream daemon if it is running
		//this will not reset the static directories
		//requires the repository to be set up and locks it for the duration of the operation
		//does not compile the game
		//runs asyncronously
		//returns true if the operation began, false if it could not start
		[OperationContract]
		bool Initialize();

		//Does all the necessary actions to take the revision currently in the repository
		//and compile it to be run on the next server reboot
		//requires byond to be set up and the compiler to be initialized
		//runs asyncronously
		//returns true if the operation began, false if it could not start
		[OperationContract]
		bool Compile();

		//Returns the current compiler status
		[OperationContract]
		TGCompilerStatus GetStatus();

		//null means the operation succeeded
		//will return an error message otherwise
		//this returns to normal after being checked or starting an operation
		string CompileError();
	}
}
