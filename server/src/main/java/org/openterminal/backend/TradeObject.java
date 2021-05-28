package org.openterminal.backend;

import com.fasterxml.jackson.annotation.JsonFormat;

import java.sql.Date;

public class TradeObject {
    private Integer volume;
    private Float price;
    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "dd/MM/yyyy HH:mm:ss a XXX")
    private Date date;

    public TradeObject() {}

    public TradeObject(Integer volume, Float hPrice, Date date) {
        super();
        this.volume = volume;
        this.price = hPrice;
        this.date = date;
    }

    public boolean Equals(TradeObject x) {
        return x.volume.equals(this.volume) && x.price.equals(this.price) && x.date.equals(this.date);
    }

    public Integer getVolume() {
        return volume;
    }

    public void setVolume(Integer volume) {
        this.volume = volume;
    }

    public Float getPrice() {
        return price;
    }

    public void setPrice(Float price) {
        this.price = price;
    }

    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "dd/MM/yyyy HH:mm:ss a XXX", timezone = "EST")
    public Date getDate() {
        return date;
    }

    public void setDate(Date date) {
        this.date = date;
    }
}
