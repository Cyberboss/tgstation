using System.Collections.Generic;
using System.ServiceModel;

namespace TGServiceInterface
{
	//for managing the code repository
	[ServiceContract(CallbackContract = typeof(ITGProgressCallback))]
	public interface ITGRepository : ITGAtomic
	{
		//check if the repository is valid, if not Setup must be called
		[OperationContract]
		bool Exists();

		//Deletes whatever may be left over and clones the repo at remote and checks out branch master
		//returns null on success, error message on failure
		//uses the progress callback
		[OperationContract]
		string Setup(string remote, string branch = "master");

		//returns the sha of the current HEAD
		//if null, error will contain the error
		[OperationContract]
		string GetHead(out string error);

		//returns the name of the current branch
		//if null, error will contain the error
		[OperationContract]
		string GetBranch(out string error);

		//hard checkout the the passed branch or sha
		//returns null on success, error message on failure
		//uses the progress callback
		[OperationContract]
		string Checkout(string branchorsha)

		//Fetches the origin and hard resets the current branch
		//returns null on success, error message on failure
		//uses the progress callback
		[OperationContract]
		string Update();

		//Hard resets the current branch
		//returns null on success, error message on failure
		//uses the progress callback
		[OperationContract]
		string Reset();

		//Merges the pull request number if the remote is a github repository
		//returns null on success, error message on failure
		//uses the progress callback
		[OperationContract]
		string MergePullRequest(int PRnumber);

		//Returns a list of PR# -> Sha of the currently merged pull requests
		//returns null on failure and error will be set
		[OperationContract]
		IDictionary<int, string> MergedPullRequests(out string error);
	}
}
