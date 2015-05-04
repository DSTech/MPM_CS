using System;
using System.IO;
using System.Threading.Tasks;
using MPM.Core.FileSystem;

namespace MPM.Core.Instances {
	/// <summary>
	/// Performs the effects of a File Declaration.
	/// </summary>
	public interface IInstallationOperation {
		//TODO: Define such that all types of File Declaration are supported, and the operation can be easily added to or removed from an index or filesystem.
		//ROLE: Create or take ownership of index entries and metadata
		//	Multiple index entries may be created by or result from a single operation or declaration
		//	The minimum creation or takeover is one complete index entry
		//		As the minimum for takeover is one entry, each file should represent at least one entry
		//		Archive files which may be modified in pieces should be tracked as multiple index entries
		//			This means that file declarations that create a partially-modifiable object must create multiple indexed parts associated with the parent file
		//ROLE: Verify owned index entries
		//	Readdition of failed validants should be an option provided by this role
		//ROLE: Remove index entries
		//	This should only occur if no entries created or previously-owned by this package have taken over by another, dependent mod
		//		If another package prevents this, it must be removed beforehand. Should the contrary situation arise, the result should be a fatal error
		//			This results in the update process consisting of removal of dependents and this, then installation of this-new and re-addition of dependents
		//It is assumed that files unclaimed by an index entry are to be removed entirely
		//Index entries may be wildcard, and as such are to be treated as a single file.
		//TODO: What about with overlapping file declarations, such as solid-in-wildcard scenarios?
	}
}
