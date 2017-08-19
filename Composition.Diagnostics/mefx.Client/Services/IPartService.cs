namespace mefx.Client.Services
{
	using System;
	using System.IO;
	using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

	public interface IPartService
	{
        void PromptForParts(Action<IEnumerable<ComposablePartCatalog>> callback);
	}
}