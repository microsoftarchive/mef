namespace mefx.Client.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Primitives;
using System.Windows.Input;

    public class ImportInfo : NotifyObject
    {
        private ImportDefinitionInfo _importDefinitionInfo;
		PartInfo _partInfo;

        public ImportInfo(ImportDefinitionInfo importDefinitionInfo, PartInfo partInfo)
        {
            this._importDefinitionInfo = importDefinitionInfo;
			this._partInfo = partInfo;
        }

        public string DisplayName
        {
            get
            {
                return CompositionElementTextFormatter.DisplayCompositionElement(this._importDefinitionInfo.ImportDefinition);
            }
        }

        public string ExceptionText
        {
            get
            {
                string exception = this._importDefinitionInfo.Exception != null ?
                    this._importDefinitionInfo.Exception.ToString() : String.Empty;

                return exception;
            }
        }

		public Exception ActualException
		{
			get { return this._importDefinitionInfo.Exception; }
		}

		public ImportCardinality ImportCardinality
		{
			get { return _importDefinitionInfo.ImportDefinition.Cardinality; }
		}

		//public int MatchCount
		//{
		//    get { return _importDefinitionInfo.IsBroken
		//}

		public bool HasUnsuitableExportDefinitions
		{
			get
			{
				return UnsuitableExportDefinitions.Count() > 0;
			}
		}

        public IEnumerable<UnsuitableExportInfo> UnsuitableExportDefinitions
        {
            get
            {
                var results = from x in this._importDefinitionInfo.UnsuitableExportDefinitions
                              select new UnsuitableExportInfo(x, _partInfo);

                return results;
            }
        }

        public bool HasMatchedExportDefinitions
        {
            get
            {
                return MatchedExportDefinitions.Count() > 0;
            }
        }

        public IEnumerable<ExportInfo> MatchedExportDefinitions
        {
            get
            {
                return this._importDefinitionInfo.Actuals.Select(e => new ExportInfo(e));
            }
        }
    }

    public class UnsuitableExportInfo
    {
        private UnsuitableExportDefinitionInfo _unsuitableExportDefinitionInfo;
		private PartInfo _partInfo;

		private ICommand _showPartCommand;

		public UnsuitableExportInfo(UnsuitableExportDefinitionInfo unsuitableExportDefinitionInfo, PartInfo partInfo)
        {
            this._unsuitableExportDefinitionInfo = unsuitableExportDefinitionInfo;
			this._partInfo = partInfo;

			_showPartCommand = new RelayCommand(p => ShowPart());
        }

        public string DisplayName
        {
            get
            {
                return CompositionElementTextFormatter.DescribeCompositionElement(this._unsuitableExportDefinitionInfo.ExportDefinition);
            }
        }

        public IEnumerable<UnsuitableExportDefinitionIssue> Issues
        {
            get
            {
                return this._unsuitableExportDefinitionInfo.Issues;
            }
        }

		public void ShowPart()
		{
            var partToShow = _partInfo.MainViewModel.GetPartViewModel(_unsuitableExportDefinitionInfo.PartDefinition);
            //partToShow.IsSelected = true;
			_partInfo.MainViewModel.ShowPart(partToShow);
		}

		public ICommand ShowPartCommand
		{
			get { return _showPartCommand; }
		}
    }
}
