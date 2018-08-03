import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";

import TopAppBar from './TopAppBar';
import BottomBar from './BottomBar';
import People from './People';

const styles = {
  bottom: {
    position: 'fixed',
    bottom: 0,
  }
};

class Home extends Component {
    constructor() {
      super();
      this.state = {
        people: [],
      };
    }

    componentDidMount() {
      const server = 'http://localhost:1337';
      fetch(`${server}/person`)
      .then(results => results.json())
      .then(data => {
        this.setState({
          people: data.data,
        });
      });
    }

    render() {
      const { classes } = this.props;

      return (
        <Router>
          <div>
            <TopAppBar />
              <Route path="/home" render={() => <People people={this.state.people}/>} />
              {/* <Route path="/scan" component={Scan} />
              <Route path="/report" component={Report} /> */}
            <BottomBar />
          </div>
        </Router>
      );
    }
  }
  
  export default withStyles(styles)(Home);