import React, { Component } from 'react';
import { Admin, Resource } from 'react-admin';

import restProvider from './sails-rest-provider';
import logo from './logo.svg';
import './App.css';

import { PersonList, PersonCreate, PersonEdit } from './persons';
import { CameraList, CameraCreate, CameraEdit } from './cameras';

const App = () => (
  <Admin
    dataProvider={restProvider('http://localhost:1337')}
    title="Kashef Dashboard"
  >
    <Resource name="person" list={PersonList} create={PersonCreate} edit={PersonEdit} />
    <Resource name="camera" list={CameraList} create={CameraCreate} edit={CameraEdit} />
  </Admin>
);

export default App;
