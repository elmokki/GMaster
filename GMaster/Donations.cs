﻿namespace GMaster
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Annotations;
    using Windows.Services.Store;

    public class Donations : INotifyPropertyChanged
    {
        private bool canDonate = true;
        private StoreContext context;
        private bool inProgress;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanDonate
        {
            get
            {
                return canDonate;
            }

            set
            {
                if (value == canDonate)
                {
                    return;
                }

                canDonate = value;
                OnPropertyChanged();
            }
        }

        public bool InProgress
        {
            get
            {
                return inProgress;
            }

            set
            {
                if (value == inProgress)
                {
                    return;
                }

                inProgress = value;
                OnPropertyChanged();
            }
        }

        public async Task<bool> PurchaseAddOn(string storeId)
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }

            InProgress = true;
            try
            {
                int errorTries = 3;
                do
                {
                    var result = await context.RequestPurchaseAsync(storeId);
                    switch (result.Status)
                    {
                        case StorePurchaseStatus.AlreadyPurchased:
                            await Consume(storeId);
                            break;

                        case StorePurchaseStatus.Succeeded:
                            await Consume(storeId);
                            return true;

                        default:
                            Debug.WriteLine(result.ExtendedError);
                            Log.Error(result.ExtendedError);
                            break;
                    }
                }
                while (errorTries-- > 0);

                CanDonate = false;

                return false;
            }
            finally
            {
                InProgress = false;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task<bool> Consume(string storeId)
        {
            var result = await context.ReportConsumableFulfillmentAsync(storeId, 1, Guid.NewGuid());

            if (result.Status == StoreConsumableStatus.Succeeded)
            {
                return true;
            }

            Log.Error(result.ExtendedError);
            return false;
        }
    }
}
