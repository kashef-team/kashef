import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import BottomNavigation from '@material-ui/core/BottomNavigation';
import BottomNavigationAction from '@material-ui/core/BottomNavigationAction';
import HomeIcon from '@material-ui/icons/Home';
import ReportIcon from '@material-ui/icons/Report';
import CameraIcon from '@material-ui/icons/Camera';
import { Route } from 'react-router-dom';

const styles = {
    bottom: {
        position: 'fixed',
        bottom: 0,
        width: '100%',
      }
};

class BottomBar extends React.Component {
    state = {
        value: 0,
    };

    handleChange = (event, value) => {
        this.setState({ value });
    };

    render() {
        const { classes } = this.props;
        const { value } = this.state;

        return (
            <Route render={({ history}) => (
                <div className={classes.bottom}>
                    <BottomNavigation
                        value={value}
                        onChange={ (event, value) => {
                            this.setState({ value });
                            switch(value) {
                                case 0:
                                    history.push('/');
                                break;
                                case 1:
                                    history.push('/scan');
                                break;
                                case 2:
                                    history.push('/report');
                                break;
                            }
                        }}
                        showLabels
                    >
                        <BottomNavigationAction label="Home" icon={<HomeIcon />} />
                        <BottomNavigationAction label="Scan" icon={<CameraIcon />} />
                        <BottomNavigationAction label="Report" icon={<ReportIcon />} />
                    </BottomNavigation>
                </div>
            )} />
        );
    };
}

BottomBar.propTypes = {
    classes: PropTypes.object.isRequired,
};
  
export default withStyles(styles)(BottomBar);