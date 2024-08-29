package com.wit.example.UI;

public class ListItem {
    private String title;
    private String data;

    public ListItem(String title, String data) {
        this.title = title;
        this.data = data;
    }

    public String getTitle() {
        return title;
    }

    public String getData() {
        return data;
    }

    public void setData(String data){
        this.data = data;
    }
}
