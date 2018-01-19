import { NgModule } from "@angular/core";
import { Repository } from "./repository";
import { Cart } from "./cart.model";
import { Order } from "./order.model";

//decorator like atribute in C#

@NgModule({
    providers: [Repository, Cart, Order]  //used to register classes for dependency injection
})
export class ModelModule { }