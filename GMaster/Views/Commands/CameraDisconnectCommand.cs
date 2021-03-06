namespace GMaster.Views.Commands
{
    using System.Diagnostics;
    using Camera;
    using Tools;

    public class CameraDisconnectCommand : AbstractSimpleParameterCommand
    {
        public override async void Execute(object parameter)
        {
            var lumix = parameter as Lumix;
            Debug.Assert(lumix != null, "lumix != null");
            await lumix.Disconnect();
        }
    }
}