namespace mefx.Client.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;
    using System.Windows.Input;
    using mefx.Client.Services;
    using Microsoft.ComponentModel.Composition.Diagnostics;
    using System;
    using System.Diagnostics;

    [Export(typeof(MainViewModel))]
    public sealed class MainViewModel : NotifyObject
    {
        private IPartService fileService;
        private CompositionContainer _container;

        private ICommand openFilesCommand;
        private ICommand resetCommand;

        private CompositionInfo compositionInfo;
        private AggregateCatalog _aggregateCatalog;

        private Dictionary<PartDefinitionInfo, PartInfo> _partViewModelMap;

        [ImportingConstructor]
        public MainViewModel(IPartService fileService)
        {
            this.fileService = fileService;

            _aggregateCatalog = new AggregateCatalog();

            _partViewModelMap = new Dictionary<PartDefinitionInfo, PartInfo>();

            this.partDefinitions = new CollectionViewSource();
            this.PartDefinitions.SortDescriptions.Add(new SortDescription("IsRejected", ListSortDirection.Descending));


            openFilesCommand = new RelayCommand(p => OpenFiles());
            resetCommand = new RelayCommand(p => Reset());


        }

        private readonly CollectionViewSource partDefinitions;
        public CollectionViewSource PartDefinitions
        {
            get
            {
                return partDefinitions;
            }

            //set
            //{
            //    partDefinitions = value;
            //    RaisePropertyChanged("PartDefinitions");
            //}
        }

        public ICommand OpenFilesCommand
        {
            get { return openFilesCommand; }
        }

        public ICommand ResetCommand
        {
            get { return resetCommand; }
        }

        public void OpenFiles()
        {
            this.fileService.PromptForParts(
                result =>
                {
                    this.LoadParts(result);
                });
        }

        private void ResetContainer()
        {
            //  Dispose any previous container, so that modifying aggregate catalog
            //  won't cause recomposition failures
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }

        private void LoadParts(IEnumerable<ComposablePartCatalog> catalogs)
        {
            if (catalogs != null && catalogs.Count() > 0)
            {
                ResetContainer();

                if (catalogs != null)
                {
                    foreach (var catalog in catalogs)
                    {
                        _aggregateCatalog.Catalogs.Add(catalog);
                    }
                }


                this.RefreshOutput();
            }
        }

        private void Reset()
        {
            _aggregateCatalog = new AggregateCatalog();

            this.RefreshOutput();
        }

        private void RefreshOutput()
        {
            ResetContainer();
            _container = new CompositionContainer(_aggregateCatalog);
            {
                this.compositionInfo = new CompositionInfo(_aggregateCatalog, _container);

                if (this.compositionInfo != null)
                {
                    StringBuilder builder = new StringBuilder();

                    //var definitions = from x in this.compositionInfo.PartDefinitions
                    //                  select new PartInfo(x, this);

                    _partViewModelMap = this.compositionInfo.PartDefinitions.ToDictionary(pd => pd, pd => new PartInfo(pd, this));

                    //this.partDefinitions.Source = definitions;
                    this.partDefinitions.Source = _partViewModelMap.Values;
                    this.partDefinitions.View.MoveCurrentToFirst();
                }
            }
        }

        public PartInfo GetPartViewModel(PartDefinitionInfo info)
        {
            return _partViewModelMap[info];
        }


        public Action<PartInfo> ShowPart { get; set; }
        //public void ShowPart(PartInfo partInfo)
        //{
        //    //this.partDefinitions.View.MoveCurrentTo(partInfo);

        //    //new System.Windows.Controls.ListBox().SelectedItem;


        //}

        private enum PartSelection : int
        {
            None = 0,
            All = 1,
            AllRejectedParts = 2,
            RejectedRootCauses = 3
        }
    }

    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion // ICommand Members

        public void RequerySuggested()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}