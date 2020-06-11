using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Uber_Rider.DataModels;
using System.Collections.Generic;
using Android.Graphics;
using LetsRide;

namespace Uber_Rider.Adapters
{
   public class VehicleAdapter : RecyclerView.Adapter
    {
        public event EventHandler<VehicleAdapterClickEventArgs> ItemClick;
        public event EventHandler<VehicleAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<VehicleAdapterClickEventArgs> SelectItemClick;

        List<VehicleType> Items;

        public VehicleAdapter(List<VehicleType> data)
        {
            Items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.vehicletype, parent, false);
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new VehicleAdapterViewHolder(itemView, OnClick, OnLongClick, OnSelectClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            VehicleType vehicle = Items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as VehicleAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.vehicleName.Text = vehicle.VehicleName;
            holder.vehicleDescription.Text = vehicle.VehicleDescription;
            holder.categoryName.Text = vehicle.CategoryName;
            holder.cost.Text = "Minimum Rs." + vehicle.BaseFare.ToString();
            holder.availableSeats.Text = vehicle.AvailableSeats.ToString() + " seats";
            holder.vehicleImage.SetImageBitmap(vehicle.Image);
        }

        public override int ItemCount => Items.Count;

        void OnClick(VehicleAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(VehicleAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnSelectClick(VehicleAdapterClickEventArgs args) => SelectItemClick?.Invoke(this, args);

    }

    public class VehicleAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView vehicleName { get; set; }
        public ImageView vehicleImage { get; set;}
        public TextView vehicleDescription { get; set; }
        public TextView cost { get; set; }
        public Button selectButton { get; set; }
        public TextView categoryName { get; set; }
        public TextView availableSeats { get; set; }




        public VehicleAdapterViewHolder(View itemView, Action<VehicleAdapterClickEventArgs> clickListener,
                            Action<VehicleAdapterClickEventArgs> longClickListener, Action<VehicleAdapterClickEventArgs> selectClickListener) : base(itemView)
        {
            //TextView = v;
            vehicleName = (TextView)itemView.FindViewById(Resource.Id.vehicleName);
            vehicleDescription = (TextView)itemView.FindViewById(Resource.Id.vehicleDescription);
            categoryName = (TextView)itemView.FindViewById(Resource.Id.category);
            cost = (TextView)itemView.FindViewById(Resource.Id.cost);
            // selectButton = (Button)itemView.FindViewById(Resource.Id.selected);
            availableSeats = (TextView)itemView.FindViewById(Resource.Id.availableSeats);
            vehicleImage = (ImageView)itemView.FindViewById(Resource.Id.vehicleImage);

            itemView.Click += (sender, e) => clickListener(new VehicleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new VehicleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            //  selectButton.Click += (sender, e) => selectClickListener(new VehicleAdapterClickEventArgs() { View = itemView, Position = AdapterPosition });

        }
    }

    public class VehicleAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}