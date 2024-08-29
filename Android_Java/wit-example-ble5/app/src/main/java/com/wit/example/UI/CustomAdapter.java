package com.wit.example.UI;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import com.wit.example.R;

import java.util.List;

public class CustomAdapter extends ArrayAdapter<ListItem> {

    public CustomAdapter(Context context, List<ListItem> items) {
        super(context, 0, items);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        // Get the data item for this position
        ListItem item = getItem(position);

        // Check if an existing view is being reused, otherwise inflate the view
        if (convertView == null) {
            convertView = LayoutInflater.from(getContext()).inflate(R.layout.list_item, parent, false);
        }

        // Lookup view for data population
        TextView text1 = convertView.findViewById(R.id.text1);
        TextView text2 = convertView.findViewById(R.id.text2);

        // Populate the data into the template view using the data object
        text1.setText(item.getTitle());
        text2.setText(item.getData());

        // Return the completed view to render on screen
        return convertView;
    }
}
