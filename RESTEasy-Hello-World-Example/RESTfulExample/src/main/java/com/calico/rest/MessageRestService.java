package com.calico.rest;

import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.core.Response;

import org.json.simple.JSONObject;

import com.calico.app.Utils;

//http://localhost:8080/calico/rest/message/fecha
@Path("/message")
public class MessageRestService {

	@GET
	@Path("/{param}")
	public Response printMessage(@PathParam("param") String msg) {

		String result = "Restful example : " + msg;
		String json = Utils.stringToJson();
		return Response.status(200).entity(json).build();

	}

}